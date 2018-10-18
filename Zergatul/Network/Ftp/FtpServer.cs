using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zergatul.Network.Ftp
{
    public class FtpServer
    {
        #region Public properties

        public string Greeting { get; set; } = "Zergatul FTP server";

        #endregion

        private IFtpFileSystemProvider _provider;
        private Socket _listener;
        private Thread _thread;
        private List<Client> _clients;

        public FtpServer(IFtpFileSystemProvider provider)
        {
            this._provider = provider;

            this._clients = new List<Client>();
        }

        public void Start(EndPoint localEP)
        {
            _listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(localEP);
            _listener.Listen(10);

            _thread = new Thread(ServerLoop);
            _thread.Start();
        }

        public void Start(int port)
        {
            Start(new IPEndPoint(IPAddress.Loopback, port));
        }

        public void Shutdown()
        {

        }

        #region Private methods

        private void ServerLoop()
        {
            while (true)
            {
                var socket = _listener.Accept();
                ProcessClient(socket);
            }
        }

        private void ProcessClient(Socket socket)
        {
            var client = new Client
            {
                Socket = socket,
                Connection = new FtpServerConnection(new NetworkStream(socket))
            };
            lock (_clients)
                _clients.Add(client);

            client.Thread = new Thread(ClientLoop);
            client.Thread.Start(client);
        }

        private void ClientLoop(object obj)
        {
            var client = (Client)obj;

            client.Connection.WriteReply(FtpReplyCode.ServiceReadyForNewUser, Greeting);
            client.State = ClientState.Greeting;

            while (true)
            {
                client.Connection.ReadNextCommand(out string command, out string param);
                switch (command)
                {
                    case FtpCommands.OPTS:
                        client.Connection.WriteReply(FtpReplyCode.CommandOkay);
                        break;

                    case FtpCommands.PASS:
                        if (client.State == ClientState.WaitPass)
                        {
                            client.Connection.WriteReply(FtpReplyCode.UserLoggedIn);
                            client.State = ClientState.LoggedIn;
                        }
                        else
                        {
                            client.Connection.WriteReply(FtpReplyCode.BadSequence);
                        }
                        break;

                    case FtpCommands.PWD:
                        break;

                    case FtpCommands.SYST:
                        client.Connection.WriteReply(FtpReplyCode.SystemType, "win64");
                        break;

                    case FtpCommands.USER:
                        if (client.State == ClientState.Greeting)
                        {
                            client.Connection.WriteReply(FtpReplyCode.UserNameOkayNeedPassword);
                            client.State = ClientState.WaitPass;
                        }
                        else
                        {
                            client.Connection.WriteReply(FtpReplyCode.BadSequence);
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        #region Nested classes

        private class Client
        {
            public Socket Socket;
            public FtpServerConnection Connection;
            public Thread Thread;
            public ClientState State;
        }

        private enum ClientState
        {
            Greeting,
            WaitPass,
            LoggedIn
        }

        #endregion
    }
}