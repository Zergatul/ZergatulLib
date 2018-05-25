using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Proxy;
using Zergatul.Network.Tls;

namespace Zergatul.Network.Smtp
{
    public class SmtpConnection
    {
        private static readonly byte[] _endOfMailData = Encoding.ASCII.GetBytes(Constants.TelnetEndOfLine + "." + Constants.TelnetEndOfLine);

        #region Private fields

        private TcpClient _tcpClient;
        private Stream _stream;
        private SmtpControlStreamReader _reader;
        private ITlsProvider _tlsProvider;

        #endregion

        #region Public properties

        public ProxyBase Proxy { get; set; }

        public int ConnectTimeout { get; set; } = -1;
        public int ReadTimeout { get; set; } = -1;
        public int WriteTimeout { get; set; } = -1;
        public int CommandTimeout { get; set; } = -1;

        /// <summary>
        /// All comunication via control connection will be logged here
        /// </summary>
        public TextWriter Log { get; set; }

        public string Greeting { get; private set; }
        public string ExtendedHelloResponse { get; private set; }
        public string[] SupportedCommands { get; private set; }

        #endregion

        #region .ctor

        public SmtpConnection()
            : this(new DefaultTlsProvider())
        {

        }

        public SmtpConnection(ITlsProvider tlsProvider)
        {
            if (tlsProvider == null)
                throw new ArgumentNullException(nameof(tlsProvider));

            this._tlsProvider = tlsProvider;
        }

        #endregion

        #region Public methods

        public void Connect(string host, int port)
        {
            if (Proxy != null)
            {
                Proxy.ConnectTimeout = ConnectTimeout;
                Proxy.ReadTimeout = ReadTimeout;
                Proxy.WriteTimeout = WriteTimeout;
                _tcpClient = Proxy.CreateConnection(host, port);
            }
            else
            {
                _tcpClient = new TcpClient(AddressFamily.InterNetwork);
                _tcpClient.ConnectWithTimeout(host, port, ConnectTimeout);
            }

            _stream = _tcpClient.GetStream();

            if (ReadTimeout >= 0)
                _tcpClient.ReceiveTimeout = ReadTimeout;
            if (WriteTimeout >= 0)
                _tcpClient.SendTimeout = WriteTimeout;

            _reader = new SmtpControlStreamReader(_stream);

            SetupCommandTimeout();

            var greetingReply = _reader.ReadServerReply();
            if (greetingReply.Code != SmtpReplyCode.ServiceReady)
                throw new SmtpException();

            Greeting = greetingReply.Message;
        }

        public void ExtendedHello(string domain)
        {
            SetupCommandTimeout();

            var reply = SendCommand(SmtpCommands.EHLO, domain);
            if (reply.Code != SmtpReplyCode.OK)
                throw new SmtpException();

            ExtendedHelloResponse = reply.Message;
            ParseExtendedHello();
        }

        public void StartTls(string host)
        {
            SetupCommandTimeout();

            if (!SupportedCommands.Contains(SmtpCommands.STARTTLS))
                throw new SmtpException("Not supported");

            var reply = SendCommand(SmtpCommands.STARTTLS);
            if (reply.Code != SmtpReplyCode.ServiceReady)
                throw new SmtpException();

            var tls = _tlsProvider.AuthenticateAsClient(_stream, host);
            _stream = tls;
            _reader = new SmtpControlStreamReader(tls);
        }

        public void AuthPlain(string user, string password)
        {
            SetupCommandTimeout();

            if (!SupportedCommands.Contains(SmtpCommands.AUTH + " PLAIN"))
                throw new SmtpException("Not supported");

            var reply = SendCommand(SmtpCommands.AUTH, "PLAIN " + Sasl.PlainMechanism.Encode(null, user, password));

            switch (reply.Code)
            {
                case SmtpReplyCode.AuthSucceded:
                    return;

                case SmtpReplyCode.InvalidCredentials:
                    throw new SmtpException("Invalid credentials");

                default:
                    throw new SmtpException();
            }
        }

        public void Mail(string path)
        {
            SetupCommandTimeout();

            var reply = SendCommand(SmtpCommands.MAIL, $"FROM:<{path}>");
            if (reply.Code != SmtpReplyCode.OK)
                throw new SmtpException();
        }

        public void Recipient(string path)
        {
            SetupCommandTimeout();

            var reply = SendCommand(SmtpCommands.RCPT, $"TO:<{path}>");
            if (reply.Code != SmtpReplyCode.OK)
                throw new SmtpException();
        }

        public void Data(byte[] mail)
        {
            SetupCommandTimeout();

            var reply = SendCommand(SmtpCommands.DATA);
            if (reply.Code != SmtpReplyCode.StartInput)
                throw new SmtpException();

            if (ByteArray.Contains(mail, _endOfMailData))
                throw new SmtpException("Mail data cannot contain terminator");

            byte[] buffer = ByteArray.Concat(mail, _endOfMailData);
            _stream.Write(buffer, 0, buffer.Length);

            reply = _reader.ReadServerReply();
            if (reply.Code != SmtpReplyCode.OK)
                throw new SmtpException();
        }

        public void Quit()
        {
            SetupCommandTimeout();

            var reply = SendCommand(SmtpCommands.QUIT);
            if (reply.Code != SmtpReplyCode.ChannelClosing)
                throw new SmtpException();

            _stream.Close();
            _tcpClient.Close();
        }

        #endregion

        #region Private methods

        private SmtpServerReply SendCommand(string command, string param = null)
        {
            string commandWithParam = command + (string.IsNullOrEmpty(param) ? "" : " " + param);
            var bytes = Encoding.ASCII.GetBytes(commandWithParam + Constants.TelnetEndOfLine);
            if (Log != null)
                Log.WriteLine(commandWithParam);
            _stream.Write(bytes, 0, bytes.Length);
            _reader.CheckTimeout();
            var reply = _reader.ReadServerReply();
            if (Log != null)
                Log.WriteLine(reply.Message);
            return reply;
        }

        private void SetupCommandTimeout()
        {
            if (CommandTimeout > 0)
            {
                _reader.Timer = System.Diagnostics.Stopwatch.StartNew();
                _reader.Timeout = CommandTimeout;
            }
            else
                _reader.Timeout = -1;
        }

        private void ParseExtendedHello()
        {
            var lines = ExtendedHelloResponse.Split(Constants.TelnetEndOfLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            // line[0] - domain
            var commands = new List<string>();
            foreach (var line in lines.Skip(1))
            {
                if (line == SmtpCommands.STARTTLS)
                {
                    commands.Add(SmtpCommands.STARTTLS);
                    continue;
                }

                var words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length == 0)
                    continue;

                if (words[0] == SmtpCommands.AUTH)
                {
                    foreach (var type in words.Skip(1))
                        commands.Add(SmtpCommands.AUTH + " " + type);
                }
            }

            SupportedCommands = commands.ToArray();
        }

        #endregion
    }
}