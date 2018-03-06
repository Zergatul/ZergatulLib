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
        #region Private fields

        private TcpClient _tcpClient;
        private Stream _stream;
        private SmtpControlStreamReader _reader;

        #endregion

        #region Public properties

        public ProxyBase Proxy { get; set; }

        /// <summary>
        /// All comunication via control connection will be logged here
        /// </summary>
        public TextWriter Log { get; set; }

        public string Greeting { get; private set; }
        public string ExtendedHelloResponse { get; private set; }

        #endregion

        #region Public methods

        public void Connect(string host, int port)
        {
            if (Proxy != null)
                _tcpClient = Proxy.CreateConnection(host, port);
            else
            {
                _tcpClient = new TcpClient(AddressFamily.InterNetwork);
                _tcpClient.Connect(host, port);
            }

            _stream = _tcpClient.GetStream();
            _reader = new SmtpControlStreamReader(_stream);
        }

        public void ExtendedHello(string domain)
        {
            if (Greeting == null)
            {
                var greeting = SendCommand(SmtpCommands.EHLO, domain);
                var reply = _reader.ReadServerReply();

                if (greeting.Code != SmtpReplyCode.ServiceReady)
                    throw new SmtpException();
                if (reply.Code != SmtpReplyCode.OK)
                    throw new SmtpException();

                Greeting = greeting.Message;
                ExtendedHelloResponse = reply.Message;
            }
            else
            {
                var reply = SendCommand(SmtpCommands.EHLO, domain);
                if (reply.Code != SmtpReplyCode.OK)
                    throw new SmtpException();
                ExtendedHelloResponse = reply.Message;
            }
        }

        public void StartTls(string host)
        {
            var reply = SendCommand(SmtpCommands.STARTTLS);
            if (reply.Code != SmtpReplyCode.ServiceReady)
                throw new SmtpException();

            //TlsStream tls = new TlsStream(_stream);
            //tls.AuthenticateAsClient(host);
            //_stream = tls;
            //_reader = new SmtpControlStreamReader(tls);

            var ssl = new System.Net.Security.SslStream(_stream);
            ssl.AuthenticateAsClient(host);
            _stream = ssl;
            _reader = new SmtpControlStreamReader(ssl);
        }

        public void AuthPlain(string user, string password)
        {
            string message = '\0' + user + '\0' + password;
            byte[] bytes = Encoding.UTF8.GetBytes(message);

            var reply = SendCommand(SmtpCommands.AUTH, "PLAIN " + Convert.ToBase64String(bytes));
            if (reply.Code != SmtpReplyCode.AuthSucceded)
                throw new SmtpException();
        }

        public void Mail(string path)
        {
            var reply = SendCommand(SmtpCommands.MAIL, $"FROM:<{path}>");
            if (reply.Code != SmtpReplyCode.OK)
                throw new SmtpException();
        }

        public void Recipient(string path)
        {
            var reply = SendCommand(SmtpCommands.RCPT, $"TO:<{path}>");
            if (reply.Code != SmtpReplyCode.OK)
                throw new SmtpException();
        }

        public void Data(string mail)
        {
            var reply = SendCommand(SmtpCommands.DATA);
            if (reply.Code != SmtpReplyCode.StartInput)
                throw new SmtpException();

            mail += Constants.TelnetEndOfLine + "." + Constants.TelnetEndOfLine;
            byte[] buffer = Encoding.UTF8.GetBytes(mail);
            _stream.Write(buffer, 0, buffer.Length);

            reply = _reader.ReadServerReply();
            if (reply.Code != SmtpReplyCode.OK)
                throw new SmtpException();
        }

        public void Quit()
        {
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
            var reply = _reader.ReadServerReply();
            if (Log != null)
                Log.WriteLine(reply.Message);
            return reply;
        }

        #endregion
    }
}