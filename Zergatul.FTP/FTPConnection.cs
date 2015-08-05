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
    public class FtpConnection
    {
        private static readonly Regex DataPortRegex = new Regex(@"\((\d+),(\d+),(\d+),(\d+),(\d+),(\d+)\)");

        private TcpClient _tcpClient;
        private string _host;
        private Stream _commandStream;
        private byte[] _readBuffer, _messageBuffer, _passiveBuffer;
        private bool _passive, _tls;
        private IPEndPoint _passiveIPEndPoint;
        private IPAddress _resolvedHost;

        public X509CertificateCollection X509CertificateCollection { get; set; }
        public ProxyBase Proxy { get; set; }

        public FtpConnection()
        {
            this._readBuffer = new byte[256];
            this._messageBuffer = new byte[1024];
            this._passiveBuffer = new byte[1024];
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
            ReadFromServer();
        }

        public void Login(string user, string password)
        {
            SendCommand("USER " + user);
            ReadFromServer();
            SendCommand("PASS " + password);
            ReadFromServer();
        }

        public void EnterPassiveMode()
        {
            SendCommand("PASV");
            string response = ReadFromServer();
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

        public void List()
        {
            SendCommand("LIST");
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

        public void AuthTls()
        {
            SendCommand("AUTH TLS");
            ReadFromServer();
            _commandStream = AuthenticateStreamWithTls(_commandStream);
            _tls = true;
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

        private void SendCommand(string command)
        {
            var bytes = Encoding.ASCII.GetBytes(command + Environment.NewLine);
            Console.WriteLine(command);
            _commandStream.Write(bytes, 0, bytes.Length);
        }

        private string ReadFromServer()
        {
            int messageLength = 0;
            while (true)
            {
                int bytesRead = _commandStream.Read(_readBuffer, 0, _readBuffer.Length);
                Array.Copy(_readBuffer, 0, _messageBuffer, messageLength, bytesRead);
                messageLength += bytesRead;
                string result = Encoding.ASCII.GetString(_messageBuffer, 0, messageLength);
                if (result.EndsWith(Environment.NewLine))
                {
                    result = result.Substring(0, messageLength - Environment.NewLine.Length);
                    Console.WriteLine(result);
                    return result;
                }
            }
        }

        public void Disconnect()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                SendCommand("QUIT");
                ReadFromServer();
                _tcpClient.Close();
            }
        }
    }
}