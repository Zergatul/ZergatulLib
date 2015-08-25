using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Zergatul.Net;
using Zergatul.Net.Proxy;

namespace Zergatul.Ftp
{
    // RFCs
    // FILE TRANSFER PROTOCOL (FTP) https://tools.ietf.org/html/rfc959
    // Requirements for Internet Hosts -- Application and Support https://tools.ietf.org/html/rfc1123
    // Firewall-Friendly FTP https://tools.ietf.org/html/rfc1579
    // FTP Security Extensions https://tools.ietf.org/html/rfc2228
    // Feature negotiation mechanism for the File Transfer Protocol https://tools.ietf.org/html/rfc2389
    // FTP Extensions for IPv6 and NATs https://tools.ietf.org/html/rfc2428
    // Extensions to FTP https://tools.ietf.org/html/rfc3659
    // Securing FTP with TLS https://tools.ietf.org/html/rfc4217

    public class FtpConnection
    {
        private static readonly Regex ServerReplyRegex = new Regex(@"^(\d{3}) (.+)$");
        private static readonly Regex DataPortRegex = new Regex(@"\((\d+),(\d+),(\d+),(\d+),(\d+),(\d+)\)");
        private static readonly Regex ExPassiveModeRegex = new Regex(@"\(\|\|\|(\d+)\|\)");

        private TcpClient _tcpClient;
        private string _host;
        private Stream _commandStream;
        private byte[] _readBuffer, _passiveBuffer;
        private List<byte> _messageBytes;
        private bool _passive, _tls;
        private IPEndPoint _passiveIPEndPoint;
        private IPAddress _resolvedHost;

        private bool _asyncOperationInProcess;

        public TextWriter Log { get; set; }
        public X509CertificateCollection X509CertificateCollection { get; set; }
        public ProxyBase Proxy { get; set; }
        public bool ThrowExceptionOnServerErrors { get; set; }

        public string Greeting { get; private set; }
        public string OperatingSystem { get; private set; }
        public FtpTransferMode TransferMode { get; private set; }

        public FtpConnection()
        {
            this._readBuffer = new byte[256];
            this._passiveBuffer = new byte[1024];
            this._messageBytes = new List<byte>(256);
            this.X509CertificateCollection = new X509CertificateCollection();
            this.ThrowExceptionOnServerErrors = false;
        }

        public void Connect(string host, int port)
        {
            _host = host;
            if (Proxy != null)
                _tcpClient = CreateProxyConnection(host, port);
            else
                _tcpClient = new TcpClient(host, port);
            if (!_tcpClient.Connected)
                throw new Exception("");
            _commandStream = _tcpClient.GetStream();
            Greeting = ReadFromServer();
            TransferMode = FtpTransferMode.Stream;
        }

        #region Access control

        public void Login(string user, string password)
        {
            string response = SendCommand(FtpCommands.USER, user);
            var reply = ParseServerReply(SendCommand(FtpCommands.PASS, password));
            if (reply.Code == FtpReplyCode.NotLoggedIn)
                throw new FtpException(reply.Message, reply.Code);
        }

        public void Logout()
        {
            if (this._asyncOperationInProcess)
                throw new Exception();

            SendCommand(FtpCommands.QUIT);
            _tcpClient.Close();
        }

        public void AuthTls()
        {
            SendCommand(FtpCommands.AUTH, "TLS");
            _commandStream = AuthenticateStreamWithTls(_commandStream);
            _tls = true;
        }

        // ACCT, SMNT, REIN

        #endregion

        #region Current directory

        public void ChangeWorkingDirectory(string dir)
        {
            SendCommand(FtpCommands.CWD, dir);
        }

        public void ChangeToParentDirectory()
        {
            SendCommand(FtpCommands.CDUP);
        }

        // PWD

        #endregion

        #region Transfer parameter

        public void EnterPassiveMode()
        {
            string response = SendCommand(FtpCommands.PASV);
            Match m = DataPortRegex.Match(response);
            if (!m.Success)
                throw new Exception("");

            var ip = new IPAddress(new byte[]
                {
                    byte.Parse(m.Groups[1].Value),
                    byte.Parse(m.Groups[2].Value),
                    byte.Parse(m.Groups[3].Value),
                    byte.Parse(m.Groups[4].Value)
                });
            int port = (byte.Parse(m.Groups[5].Value) << 8) | byte.Parse(m.Groups[6].Value);
            _passiveIPEndPoint = new IPEndPoint(ip, port);
            _passive = true;
        }

        public void EnterPassiveModeEx()
        {
            var reply = ParseServerReply(SendCommand(FtpCommands.EPSV));
            var m = ExPassiveModeRegex.Match(reply.Message);
            if (!m.Success)
                throw new Exception("parse fail");

            int port = int.Parse(m.Groups[1].Value);
            _passiveIPEndPoint = new IPEndPoint(Dns.GetHostEntry(_host).AddressList[0], port);
            _passive = true;
        }

        public void EnterActiveMode(IPAddress address, int port)
        {
            string parameter = string.Join(",", address.GetAddressBytes().Concat(new byte[] { (byte)(port / 256), (byte)(port % 256) }).Select(b => b.ToString()));
            string response = SendCommand(FtpCommands.PORT, parameter);
        }

        public void EnterActiveModeEx(IPAddress address, int port)
        {
            char delimiter = '|';
            string parameter;
            if (address.AddressFamily == AddressFamily.InterNetwork)
                parameter = delimiter + FtpNetworkProtocol.IPv4 + delimiter + address.ToString() + delimiter + port + delimiter;
            else
                if (address.AddressFamily == AddressFamily.InterNetworkV6)
                    parameter = delimiter + FtpNetworkProtocol.IPv6 + delimiter + address.ToString() + delimiter + port + delimiter;
                else
                    throw new NotImplementedException(address.AddressFamily + " not implemented");
            string response = SendCommand(FtpCommands.EPRT, parameter);
        }

        public void SetTransferMode(FtpTransferMode mode)
        {
            string param = null;
            if (mode == FtpTransferMode.Stream)
                param = "S";
            if (mode == FtpTransferMode.Block)
                param = "B";
            if (mode == FtpTransferMode.Compressed)
                param = "C";
            SendCommand(FtpCommands.MODE, param);
            TransferMode = mode;
        }

        public void SetRepresentationType(string type, string parameter)
        {
            string p = type;
            if (!string.IsNullOrEmpty(parameter))
                p += " " + parameter;
            SendCommand(FtpCommands.TYPE, p);
        }

        // TYPE, STRU

        #endregion

        #region Directory/Files commands

        public List<byte> RetrieveFile(string file)
        {
            SendCommand(FtpCommands.RETR, file, true);

            var stream = CreateDataConnection();

            var bytes = new FtpStreamReader(stream, TransferMode).ReadToEnd();
            ReadFromServer();
            return bytes;
        }

        public List<byte> RetrieveFile(string file, int from)
        {
            SendCommand(FtpCommands.REST, from.ToString());
            SendCommand(FtpCommands.RETR, file, true);

            var stream = CreateDataConnection();

            var bytes = new FtpStreamReader(stream, TransferMode).ReadToEnd();
            ReadFromServer();
            return bytes;
        }

        public Task RetrieveFileAsync(string file, Stream destination, CancellationToken cancellationToken = default(CancellationToken), IProgress<long> progress = null)
        {
            Thread.Sleep(1500);
            return Task.Run(() =>
                {
                    this._asyncOperationInProcess = true;
                    try
                    {
                        SendCommand(FtpCommands.RETR, file, true);

                        var stream = CreateDataConnection();
                        new FtpStreamReader(stream, TransferMode).ReadToStreamAsync(destination, cancellationToken, progress);
                        ReadFromServer();
                    }
                    finally
                    {
                        this._asyncOperationInProcess = false;
                    }
                }, cancellationToken);
        }

        public void StoreFile(string file, Stream content)
        {
            SendCommand(FtpCommands.STOR, file, true);

            var stream = CreateDataConnection();
            new FtpStreamWriter(stream, TransferMode).Write(content);

            ReadFromServer();
        }

        public void StoreFile(string file, byte[] content)
        {
            SendCommand(FtpCommands.STOR, file, true);

            var stream = CreateDataConnection();
            new FtpStreamWriter(stream, TransferMode).Write(content);

            ReadFromServer();
        }

        public void AppendFile(string file, Stream content)
        {
            SendCommand(FtpCommands.APPE, file, true);

            var stream = CreateDataConnection();
            new FtpStreamWriter(stream, TransferMode).Write(content);

            ReadFromServer();
        }

        public void AppendFile(string file, byte[] content)
        {
            SendCommand(FtpCommands.APPE, file, true);

            var stream = CreateDataConnection();
            new FtpStreamWriter(stream, TransferMode).Write(content);

            ReadFromServer();
        }

        public void DeleteFile(string file)
        {
            SendCommand(FtpCommands.DELE, file);
        }

        public void RenameFile(string oldName, string newName)
        {
            SendCommand(FtpCommands.RNFR, oldName);
            SendCommand(FtpCommands.RNTO, newName);
        }

        public void MakeDirectory(string dir)
        {
            SendCommand(FtpCommands.MKD, dir);
        }

        public void RemoveDirectory(string dir)
        {
            SendCommand(FtpCommands.RMD, dir);
        }

        public void List()
        {
            SendCommand(FtpCommands.LIST, null, true);

            var stream = CreateDataConnection();

            var bytes = new FtpStreamReader(stream, TransferMode).ReadToEnd();
            ReadFromServer();
            if (Log != null)
                Log.WriteLine(Encoding.ASCII.GetString(bytes.ToArray()));
        }

        public void NameList()
        {
            SendCommand(FtpCommands.NLST, null, true);

            var stream = CreateDataConnection();

            var bytes = new FtpStreamReader(stream, TransferMode).ReadToEnd();
            ReadFromServer();
            if (Log != null)
                Log.WriteLine(Encoding.ASCII.GetString(bytes.ToArray()));
        }

        // STOU, ALLO, REST, ABOR

        #endregion

        #region Other commands

        public void System()
        {
            string response = SendCommand(FtpCommands.SYST);
            OperatingSystem = response;
        }

        public void Help(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
                SendCommand(FtpCommands.HELP);
            else
                SendCommand(FtpCommands.HELP, cmd);
        }

        public string Features()
        {
            var reply = ParseServerReply(SendCommand(FtpCommands.FEAT));
            return reply.Message;
        }

        // SITE, STAT, HELP, NOOP

        #endregion

        #region Private helper methods

        private TcpClient CreateProxyConnection(string host, int port)
        {
            if (Proxy.AllowConnectionsByDomainName)
                return Proxy.CreateConnection(_host, port);
            else
            {
                if (_resolvedHost == null)
                    _resolvedHost = Dns.GetHostEntry(_host).AddressList[0];
                return Proxy.CreateConnection(_resolvedHost, port);
            }
        }

        private Stream AuthenticateStreamWithTls(Stream innerStream)
        {
            SslStream stream = new SslStream(innerStream);
            stream.AuthenticateAsClient(
                this._host,
                this.X509CertificateCollection,
                SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12,
                true);
            return stream;
        }

        private Stream CreateDataConnection()
        {
            if (!_passive)
                throw new NotImplementedException("Active mode not implemented");

            TcpClient passiveConnection;
            if (Proxy != null)
                passiveConnection = Proxy.CreateConnection(_passiveIPEndPoint.Address, _passiveIPEndPoint.Port);
            else
            {
                passiveConnection = new TcpClient();
                passiveConnection.Connect(_passiveIPEndPoint.Address, _passiveIPEndPoint.Port);
            }

            Stream result = passiveConnection.GetStream();

            if (_tls)
                result = AuthenticateStreamWithTls(result);

            return result;
        }

        private string SendCommand(string command, string param = null, bool doNotReadReply = false)
        {
            string commandWithParam = command + (string.IsNullOrEmpty(param) ? "" : " " + param);
            var bytes = Encoding.ASCII.GetBytes(commandWithParam  + Constants.TelnetEndOfLine);
            if (Log != null)
                Log.WriteLine(commandWithParam);
            _commandStream.Write(bytes, 0, bytes.Length);
            if (doNotReadReply)
                return null;
            else
                return ReadFromServer();
        }

        private string ReadFromServer()
        {
            _messageBytes.Clear();
            while (true)
            {
                int bytesRead = _commandStream.Read(_readBuffer, 0, _readBuffer.Length);
                for (int i = 0; i < bytesRead; i++)
                    _messageBytes.Add(_readBuffer[i]);
                string result = Encoding.ASCII.GetString(_messageBytes.ToArray());
                if (result.EndsWith(Constants.TelnetEndOfLine))
                {
                    // check if result multiline
                    if (result.Length > 6 && result[3] == '-')
                    {
                        var lines = result.Split(new[] { Constants.TelnetEndOfLine }, StringSplitOptions.None);
                        var lastLine = lines[lines.Length - 2];
                        if (lastLine.Length < 4 || lastLine[3] != ' ')
                            continue;
                    }
                    if (Log != null)
                        Log.Write(result);
                    return result;
                }
            }
        }

        private ServerReply ParseServerReply(string s)
        {
            var lines = s.Split(new[] { Constants.TelnetEndOfLine }, StringSplitOptions.None);
            var match = ServerReplyRegex.Match(lines[lines.Length - 2]);
            if (!match.Success)
                throw new FtpException("Cannot parse server reply.");
            return new ServerReply((FtpReplyCode)int.Parse(match.Groups[1].Value), match.Groups[2].Value);
        }

        #endregion

        #region Nested Classes

        private class ServerReply
        {
            public FtpReplyCode Code;
            public string Message;

            public ServerReply(FtpReplyCode code, string message)
            {
                this.Code = code;
                this.Message = message;
            }
        }

        #endregion
    }
}