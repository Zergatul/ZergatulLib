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
        private IPAddress _address;
        private Stream _commandStream, _baseCommandStream;
        private byte[] _readBuffer, _passiveBuffer;
        private List<byte> _messageBytes;
        private bool _passive, _tls, _dataChannelTls;
        private IPEndPoint _passiveIPEndPoint;
        private IPAddress _resolvedHost;

        private bool _asyncOperationInProcess;

        public TextWriter Log { get; set; }
        public X509CertificateCollection X509CertificateCollection { get; set; }
        public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }
        public bool PreferIPv4 { get; set; }
        public ProxyBase Proxy { get; set; }
        public bool ThrowExceptionOnServerErrors { get; set; }

        public string Greeting { get; private set; }
        public string OperatingSystem { get; private set; }
        public FtpTransferMode TransferMode { get; private set; }
        public string LastServerReply { get; private set; }

        public FtpConnection()
        {
            this._readBuffer = new byte[256];
            this._passiveBuffer = new byte[1024];
            this._messageBytes = new List<byte>(256);
            this.X509CertificateCollection = new X509CertificateCollection();
            this.ThrowExceptionOnServerErrors = true;
            this.PreferIPv4 = true;
        }

        public void Connect(string host, int port)
        {
            this._host = host;
            if (Proxy != null)
                _tcpClient = CreateProxyConnection(host, port);
            else
            {
                var ipHE = Dns.GetHostEntry(host);
                this._address = null;
                if (PreferIPv4)
                {
                    this._address = ipHE.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                    if (this._address != null)
                    {
                        _tcpClient = new TcpClient(AddressFamily.InterNetwork);
                        _tcpClient.Connect(this._address, port);
                    }
                }
                else
                {
                    this._address = ipHE.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetworkV6);
                    if (this._address != null)
                    {
                        _tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
                        _tcpClient.Connect(this._address, port);
                    }
                }
                if (this._address == null)
                {
                    this._address = ipHE.AddressList.First();
                    _tcpClient = new TcpClient(this._address.AddressFamily);
                    _tcpClient.Connect(this._address, port);
                }
            }
            if (!_tcpClient.Connected)
                throw new Exception("");
            _commandStream = _tcpClient.GetStream();
            Greeting = ReadFromServer();
            TransferMode = FtpTransferMode.Stream;
        }

        public void Connect(IPAddress address, int port)
        {
            this._address = address;
            if (Proxy != null)
                _tcpClient = Proxy.CreateConnection(address, port);
            else
            {
                _tcpClient = new TcpClient(address.AddressFamily);
                _tcpClient.Connect(address, port);
            }
            if (!_tcpClient.Connected)
                throw new Exception("");
            _commandStream = _tcpClient.GetStream();
            Greeting = ReadFromServer();
            TransferMode = FtpTransferMode.Stream;
        }

        #region Access control

        public void Login(string user, string password)
        {
            CheckStateBeforeCommand();

            ServerReply reply;
            reply = ParseServerReply(SendCommand(FtpCommands.USER, user));
            CheckReply(reply);
            reply = ParseServerReply(SendCommand(FtpCommands.PASS, password));
            CheckReply(reply);
        }

        public void Reinitialize()
        {
            CheckStateBeforeCommand();

            ServerReply reply = ParseServerReply(SendCommand(FtpCommands.REIN));
            CheckReply(reply);
        }

        public void Quit()
        {
            CheckStateBeforeCommand();

            ServerReply reply = ParseServerReply(SendCommand(FtpCommands.QUIT));
            CheckReply(reply);

            _tcpClient.Close();
        }

        #region RFC2228

        public void AuthTls()
        {
            CheckStateBeforeCommand();

            ServerReply reply = ParseServerReply(SendCommand(FtpCommands.AUTH, "TLS"));
            CheckReply(reply);

            _baseCommandStream = _commandStream;
            _commandStream = AuthenticateStreamWithTls(_commandStream);
            _tls = true;
            _dataChannelTls = false;
        }

        public void ProtectionBufferSize(int size)
        {
            CheckStateBeforeCommand();

            ServerReply reply = ParseServerReply(SendCommand(FtpCommands.PBSZ, size.ToString()));
            CheckReply(reply);
        }

        public void DataChannelProtectionLevel(FtpDataChannelProtectionLevel level)
        {
            CheckStateBeforeCommand();

            ServerReply reply = ParseServerReply(SendCommand(FtpCommands.PROT, level.ToCommand()));
            CheckReply(reply);

            _dataChannelTls = level != FtpDataChannelProtectionLevel.Clear;
        }

        public void ClearCommandChannel()
        {
            CheckStateBeforeCommand();

            ServerReply reply = ParseServerReply(SendCommand(FtpCommands.CCC));
            CheckReply(reply);

            // dispose SslStream; this will remain underlying NetworkStream opened
            _commandStream.Dispose();
            _commandStream = _baseCommandStream;
        }

        #endregion

        // ACCT, SMNT

        #endregion

        #region Current directory

        public void ChangeWorkingDirectory(string dir)
        {
            CheckStateBeforeCommand();

            ServerReply reply = ParseServerReply(SendCommand(FtpCommands.CWD, dir));
            CheckReply(reply);
        }

        public void ChangeToParentDirectory()
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.CDUP);
        }

        // PWD

        #endregion

        #region Transfer parameter

        public void EnterPassiveMode()
        {
            CheckStateBeforeCommand();

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
            CheckStateBeforeCommand();

            var reply = ParseServerReply(SendCommand(FtpCommands.EPSV));
            var m = ExPassiveModeRegex.Match(reply.Message);
            if (!m.Success)
                throw new Exception("parse fail");

            int port = int.Parse(m.Groups[1].Value);
            if (string.IsNullOrEmpty(_host))
                _passiveIPEndPoint = new IPEndPoint(_address, port);
            else
            {
                var ipv4 = Dns.GetHostEntry(_host).AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                if (ipv4 != null)
                    _passiveIPEndPoint = new IPEndPoint(ipv4, port);
                else
                    _passiveIPEndPoint = new IPEndPoint(Dns.GetHostEntry(_host).AddressList[0], port);
            }
            _passive = true;
        }

        public void EnterActiveMode(IPAddress address, int port)
        {
            CheckStateBeforeCommand();

            string parameter = string.Join(",", address.GetAddressBytes().Concat(new byte[] { (byte)(port / 256), (byte)(port % 256) }).Select(b => b.ToString()));
            string response = SendCommand(FtpCommands.PORT, parameter);
        }

        public void EnterActiveModeEx(IPAddress address, int port)
        {
            CheckStateBeforeCommand();

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
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.MODE, mode.ToCommand());
            TransferMode = mode;
        }

        public void SetRepresentationType(string type, string parameter)
        {
            CheckStateBeforeCommand();

            string p = type;
            if (!string.IsNullOrEmpty(parameter))
                p += " " + parameter;
            SendCommand(FtpCommands.TYPE, p);
        }

        // STRU

        #endregion

        #region Directory/Files commands

        public void RetrieveFile(string file, Stream destination)
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.RETR, file, true);

            var stream = CreateDataConnection();
            ReadFromServer();
            new FtpStreamReader(stream, TransferMode).ReadToStream(destination);
            stream.Close();
        }

        public void RetrieveFile(string file, int from, Stream destination)
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.REST, from.ToString());
            SendCommand(FtpCommands.RETR, file, true);

            var stream = CreateDataConnection();
            ReadFromServer();
            new FtpStreamReader(stream, TransferMode).ReadToStream(destination);
            stream.Close();
        }

        public Task RetrieveFileAsync(string file, Stream destination, CancellationToken cancellationToken = default(CancellationToken), IProgress<long> progress = null)
        {
            return Task.Run(() =>
                {
                    CheckStateBeforeCommand();

                    this._asyncOperationInProcess = true;
                    bool dataConnectionOpened = false;
                    Stream stream = null;
                    try
                    {
                        SendCommand(FtpCommands.RETR, file, true);

                        stream = CreateDataConnection();
                        ReadFromServer();
                        dataConnectionOpened = true;
                        new FtpStreamReader(stream, TransferMode).ReadToStreamAsync(destination, cancellationToken, progress);
                        stream.Close();
                        ReadFromServer();
                    }
                    catch (OperationCanceledException)
                    {
                        if (dataConnectionOpened)
                        {
                            SendCommand(FtpCommands.ABOR, null, true);
                            stream.Close();
                            ReadFromServer();
                            ReadFromServer();
                        }
                    }
                    finally
                    {
                        this._asyncOperationInProcess = false;
                    }
                }, cancellationToken);
        }

        public void StoreFile(string file, Stream content)
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.STOR, file, true);

            var stream = CreateDataConnection();
            ReadFromServer();
            new FtpStreamWriter(stream, TransferMode).Write(content);
            stream.Close();
        }

        public void StoreFile(string file, byte[] content)
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.STOR, file, true);

            var stream = CreateDataConnection();
            ReadFromServer();
            new FtpStreamWriter(stream, TransferMode).Write(content);
            stream.Close();
        }

        public void AppendFile(string file, Stream content)
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.APPE, file, true);

            var stream = CreateDataConnection();
            ReadFromServer();
            new FtpStreamWriter(stream, TransferMode).Write(content);
            stream.Close();
        }

        public void AppendFile(string file, byte[] content)
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.APPE, file, true);

            var stream = CreateDataConnection();
            ReadFromServer();
            new FtpStreamWriter(stream, TransferMode).Write(content);
            stream.Close();
        }

        public void DeleteFile(string file)
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.DELE, file);
        }

        public void RenameFile(string oldName, string newName)
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.RNFR, oldName);
            SendCommand(FtpCommands.RNTO, newName);
        }

        public void MakeDirectory(string dir)
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.MKD, dir);
        }

        public void RemoveDirectory(string dir)
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.RMD, dir);
        }

        public void List()
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.LIST, null, true);

            var stream = CreateDataConnection();
            ReadFromServer();

            var ms = new MemoryStream();
            new FtpStreamReader(stream, TransferMode).ReadToStream(ms);
            stream.Close();
            
            if (Log != null)
                Log.WriteLine(Encoding.ASCII.GetString(ms.ToArray()));
        }

        public void NameList()
        {
            CheckStateBeforeCommand();

            SendCommand(FtpCommands.NLST, null, true);

            var stream = CreateDataConnection();
            ReadFromServer();

            var ms = new MemoryStream();
            new FtpStreamReader(stream, TransferMode).ReadToStream(ms);
            stream.Close();
            
            if (Log != null)
                Log.WriteLine(Encoding.ASCII.GetString(ms.ToArray()));
        }

        // STOU, ALLO, REST

        #endregion

        #region Other commands

        public void System()
        {
            CheckStateBeforeCommand();

            string response = SendCommand(FtpCommands.SYST);
            OperatingSystem = response;
        }

        public void Help(string cmd)
        {
            CheckStateBeforeCommand();

            if (string.IsNullOrEmpty(cmd))
                SendCommand(FtpCommands.HELP);
            else
                SendCommand(FtpCommands.HELP, cmd);
        }

        public string Features()
        {
            CheckStateBeforeCommand();

            var reply = ParseServerReply(SendCommand(FtpCommands.FEAT));
            return reply.Message;
        }

        // SITE, STAT, HELP, NOOP

        #endregion

        #region Private helper methods

        private void CheckStateBeforeCommand()
        {
            if (this._asyncOperationInProcess)
                throw new Exception("Async operation currently in process.");
        }

        private void CheckReply(ServerReply reply)
        {
            if ((int)reply.Code >= 500)
            {
                if (ThrowExceptionOnServerErrors)
                    throw new FtpException(reply.Message, reply.Code);
            }
        }

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
            SslStream stream = new SslStream(innerStream, true, CertificateValidationCallback);
            stream.AuthenticateAsClient(
                this._host,
                this.X509CertificateCollection,
                SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
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
                passiveConnection = new TcpClient(_passiveIPEndPoint.Address.AddressFamily);
                passiveConnection.Connect(_passiveIPEndPoint.Address, _passiveIPEndPoint.Port);
            }

            Stream result = passiveConnection.GetStream();

            if (_tls && _dataChannelTls)
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
                    LastServerReply = result;
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