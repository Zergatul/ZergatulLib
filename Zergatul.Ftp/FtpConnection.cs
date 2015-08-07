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
using System.Threading.Tasks;
using Zergatul.Net.Proxy;

namespace Zergatul.Ftp
{
    // RFCs
    // FILE TRANSFER PROTOCOL (FTP) https://tools.ietf.org/html/rfc959
    // Requirements for Internet Hosts -- Application and Support https://tools.ietf.org/html/rfc1123
    // Firewall-Friendly FTP https://tools.ietf.org/html/rfc1579
    // FTP Security Extensions https://tools.ietf.org/html/rfc2228
    // Securing FTP with TLS https://tools.ietf.org/html/rfc4217

    public class FtpConnection
    {
        private static readonly Regex ServerReplyRegex = new Regex(@"^(\d{3}) (.+)$");
        private static readonly Regex DataPortRegex = new Regex(@"\((\d+),(\d+),(\d+),(\d+),(\d+),(\d+)\)");
        private static readonly string TelnetEndOfLine = new string(new[] { (char)0x0D, (char)0x0A });

        private TcpClient _tcpClient;
        private string _host;
        private Stream _commandStream;
        private byte[] _readBuffer, _passiveBuffer;
        private List<byte> _messageBytes;
        private bool _passive, _tls;
        private IPEndPoint _passiveIPEndPoint;
        private IPAddress _resolvedHost;

        public TextWriter Log { get; set; }
        public X509CertificateCollection X509CertificateCollection { get; set; }
        public ProxyBase Proxy { get; set; }

        public string Greeting { get; private set; }
        public string OperatingSystem { get; private set; }
        public FtpTransferMode TransferMode { get; private set; }

        public FtpConnection()
        {
            this._readBuffer = new byte[256];
            this._passiveBuffer = new byte[1024];
            this._messageBytes = new List<byte>(256);
            this.X509CertificateCollection = new X509CertificateCollection();
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

        public void Login(string user, string password)
        {
            string response = SendCommand(FtpCommands.USER, user);
            var reply = ParseServerReply(SendCommand(FtpCommands.PASS, password));
            if (reply.Code == FtpReplyCode.NotLoggedIn)
                throw new FtpException(reply.Message, reply.Code);
        }

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
        }

        public void List()
        {
            SendCommand(FtpCommands.LIST, null, true);
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
            var bytes = new List<byte>();
            Stream passiveStream = passiveConnection.GetStream();
            if (_tls)
                passiveStream = AuthenticateStreamWithTls(passiveStream);
            while (true)
            {
                int bytesRead = passiveStream.Read(_passiveBuffer, 0, _passiveBuffer.Length);
                if (bytesRead == 0)
                    break;
                if (bytesRead == _passiveBuffer.Length)
                    bytes.AddRange(_passiveBuffer);
                else
                    for (int i = 0; i < bytesRead; i++)
                        bytes.Add(_passiveBuffer[i]);
            }
            passiveConnection.Close();
            ReadFromServer();
            Console.WriteLine(Encoding.ASCII.GetString(bytes.ToArray()));
        }

        public void System()
        {
            string response = SendCommand(FtpCommands.SYST);
            OperatingSystem = response;
        }

        public void AuthTls()
        {
            SendCommand(FtpCommands.AUTH, "TLS");
            _commandStream = AuthenticateStreamWithTls(_commandStream);
            _tls = true;
        }

        public void ChangeWorkingDirectory(string dir)
        {
            SendCommand(FtpCommands.CWD, dir);
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
            SslStream stream = new SslStream(innerStream);
            stream.AuthenticateAsClient(
                this._host,
                this.X509CertificateCollection,
                SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12,
                true);
            return stream;
        }

        private string SendCommand(string command, string param = null, bool doNotReadReply = false)
        {
            string commandWithParam = command + (string.IsNullOrEmpty(param) ? "" : " " + param);
            var bytes = Encoding.ASCII.GetBytes(commandWithParam  + TelnetEndOfLine);
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
                if (result.EndsWith(TelnetEndOfLine))
                {
                    // check if result multiline
                    if (result.Length > 6 && result[3] == '-')
                    {
                        var lines = result.Split(new[] { TelnetEndOfLine }, StringSplitOptions.None);
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
            var lines = s.Split(new[] { TelnetEndOfLine }, StringSplitOptions.None);
            var match = ServerReplyRegex.Match(lines[lines.Length - 2]);
            if (!match.Success)
                throw new FtpException("Cannot parse server reply.");
            return new ServerReply((FtpReplyCode)int.Parse(match.Groups[1].Value), match.Groups[2].Value);
        }

        public void Disconnect()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                SendCommand(FtpCommands.QUIT);
                _tcpClient.Close();
            }
        }

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