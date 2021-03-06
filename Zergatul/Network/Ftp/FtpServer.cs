﻿using System;
using System.Collections.Generic;
using System.IO;
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
        public int PassivePortFrom { get; set; } = 50000;
        public int PassivePortTo { get; set; } = 51000;
        public TextWriter Log { get; set; }

        #endregion

        private IFtpFileSystemProvider _fileSystemProvider;

        private IPEndPoint _localEP;
        private Socket _listener;
        private Thread _thread;
        private List<Client> _clients;
        private HashSet<int> _occupiedPorts;

        public FtpServer(IFtpFileSystemProvider fileSystemProvider)
        {
            this._fileSystemProvider = fileSystemProvider;

            this._clients = new List<Client>();
            this._occupiedPorts = new HashSet<int>();
        }

        public void Start(IPEndPoint localEP)
        {
            _localEP = localEP;

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
            if (Log != null)
                client.Connection.Log = Log;
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
                if (client.Socket == null)
                    break;

                string command, param;
                try
                {
                    client.Connection.ReadNextCommand(out command, out param);
                }
                catch (IOException ex)
                {
                    if (Log != null)
                    {
                        Log.WriteLine(ex.Message);
                        if (ex.InnerException != null)
                            Log.WriteLine(ex.InnerException.Message);
                    }
                    break;
                }

                switch (command.ToUpper())
                {
                    case FtpCommands.ABOR:
                        OnAbor(client, param);
                        break;

                    case FtpCommands.CWD:
                        OnCwd(client, param);
                        break;

                    case FtpCommands.EPSV:
                        OnEpsv(client, param);
                        break;

                    case FtpCommands.FEAT:
                        OnFeat(client, param);
                        break;

                    case FtpCommands.OPTS:
                        client.Connection.WriteReply(FtpReplyCode.CommandOkay);
                        break;

                    case FtpCommands.PASS:
                        OnPass(client, param);
                        break;

                    case FtpCommands.PASV:
                        OnPasv(client, param);
                        break;

                    case FtpCommands.PWD:
                        OnPwd(client, param);
                        break;

                    case FtpCommands.QUIT:
                        OnQuit(client, param);
                        break;

                    case FtpCommands.REST:
                        OnRest(client, param);
                        break;

                    case FtpCommands.RETR:
                        OnRetr(client, param);
                        break;

                    case FtpCommands.SIZE:
                        OnSize(client, param);
                        break;

                    case FtpCommands.SYST:
                        OnSyst(client, param);
                        break;

                    case FtpCommands.TYPE:
                        OnType(client, param);
                        break;

                    case FtpCommands.USER:
                        OnUser(client, param);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void OnAbor(Client client, string param)
        {
            if (param != null)
                throw new InvalidOperationException();

            if (client.PassiveListener == null)
            {
                client.Connection.WriteReply(FtpReplyCode.ClosingDataConnection);
            }
            else
            {
                client.Connection.WriteReply(FtpReplyCode.ConnectionClosed);
                CloseDataConnection(client);
            }
        }

        private void OnCwd(Client client, string param)
        {
            if (client.State == ClientState.LoggedIn)
            {
                if (_fileSystemProvider.SetWorkingDirectory(param))
                    client.Connection.WriteReply(FtpReplyCode.RequestedFileActionOkay);
                else
                    client.Connection.WriteReply(FtpReplyCode.RequestedActionNotTakenFileUnavailable2);
            }
            else
                WriteBadSequence(client);
        }

        private void OnEpsv(Client client, string param)
        {
            if (string.Equals(param, "ALL", StringComparison.InvariantCultureIgnoreCase))
            {
                // Upon receipt of an EPSV ALL command, the server MUST reject all data 
                // connection setup commands other than EPSV(i.e., EPRT, PORT, PASV, et
                // al.)
                client.Connection.WriteReply(FtpReplyCode.CommandOkay);
                return;
            }

            if (param != null)
                throw new NotImplementedException();

            if (client.State == ClientState.LoggedIn)
            {
                int port = -1;
                lock (_occupiedPorts)
                {
                    for (int i = PassivePortFrom; i < PassivePortTo; i++)
                        if (!_occupiedPorts.Contains(i))
                        {
                            port = i;
                            _occupiedPorts.Add(port);
                            break;
                        }
                }
                if (port == -1)
                    throw new InvalidOperationException();

                if (client.PassiveListener != null)
                    throw new InvalidOperationException();

                client.PassivePort = port;
                client.PassiveListener = new Socket(SocketType.Stream, ProtocolType.Tcp);
                client.PassiveListener.Bind(new IPEndPoint(_localEP.Address, port));
                client.PassiveListener.Listen(1);

                var localEndPoint = (IPEndPoint)client.Socket.LocalEndPoint;
                var ip = localEndPoint.Address.GetAddressBytes();
                string message = $"Entering Extended Passive Mode (|||{port}|)";
                client.Connection.WriteReply(FtpReplyCode.EnteringExtendedPassiveMode, message);
            }
            else
                WriteBadSequence(client);
        }

        private void OnFeat(Client client, string param)
        {
            if (param != null)
                throw new InvalidOperationException();

            client.Connection.WriteFeatures(new[] { "SIZE", "MDTM", "REST STREAM" });
        }

        private void OnPass(Client client, string password)
        {
            if (client.State == ClientState.WaitPass)
            {
                client.Connection.WriteReply(FtpReplyCode.UserLoggedIn);
                client.State = ClientState.LoggedIn;
            }
            else
                WriteBadSequence(client);
        }

        private void OnPasv(Client client, string param)
        {
            if (client.State == ClientState.LoggedIn)
            {
                int port = -1;
                lock (_occupiedPorts)
                {
                    for (int i = PassivePortFrom; i < PassivePortTo; i++)
                        if (!_occupiedPorts.Contains(i))
                        {
                            port = i;
                            _occupiedPorts.Add(port);
                            break;
                        }
                }
                if (port == -1)
                    throw new InvalidOperationException();

                if (client.PassiveListener != null)
                    throw new InvalidOperationException();

                client.PassivePort = port;
                client.PassiveListener = new Socket(SocketType.Stream, ProtocolType.Tcp);
                client.PassiveListener.Bind(new IPEndPoint(_localEP.Address, port));
                client.PassiveListener.Listen(1);

                var localEndPoint = (IPEndPoint)client.Socket.LocalEndPoint;
                var ip = localEndPoint.Address.GetAddressBytes();
                string message;
                if (localEndPoint.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    message = $"({ip[0]},{ip[1]},{ip[2]},{ip[3]},{port >> 8},{port & 0xFF})";
                }
                else if (localEndPoint.Address.IsIPv4MappedToIPv6)
                {
                    message = $"({ip[12]},{ip[13]},{ip[14]},{ip[15]},{port >> 8},{port & 0xFF})";
                }
                else
                    throw new InvalidOperationException();

                client.Connection.WriteReply(FtpReplyCode.EnteringPassiveMode, message);
            }
            else
                WriteBadSequence(client);
        }

        private void OnPwd(Client client, string param)
        {
            if (param != null)
                throw new InvalidOperationException();

            if (client.State == ClientState.LoggedIn)
                client.Connection.WriteReply(FtpReplyCode.PathnameCreated, _fileSystemProvider.GetCurrentDirectory());
            else
                WriteBadSequence(client);
        }

        private void OnQuit(Client client, string param)
        {
            if (param != null)
                throw new InvalidOperationException();

            if (client.PassiveListener != null)
            {
                client.PassiveListener.Close();
                client.PassiveListener = null;

                lock (_occupiedPorts)
                    _occupiedPorts.Remove(client.PassivePort);
            }

            client.Connection.WriteReply(FtpReplyCode.ServiceClosingControlConnection);
            client.Socket.Close();
            client.Socket = null;

            lock (_clients)
                _clients.Remove(client);
        }

        private void OnRest(Client client, string param)
        {
            if (!long.TryParse(param, out long position))
                throw new InvalidOperationException();

            if (client.State == ClientState.LoggedIn)
            {
                client.Connection.WriteReply(FtpReplyCode.RequestedFileActionPendingInformation);
                client.RestartPosition = position;
            }
            else
                WriteBadSequence(client);
        }

        private void OnRetr(Client client, string filename)
        {
            if (filename == null)
                throw new InvalidOperationException();

            if (client.State == ClientState.LoggedIn)
            {
                if (client.PassiveListener == null)
                    throw new InvalidOperationException();
                if (client.DataThread != null)
                    throw new InvalidOperationException();

                var file = _fileSystemProvider.GetFile(filename);
                if (file == null)
                    throw new NotImplementedException();

                var stream = file.GetStream();
                if (stream == null)
                    throw new InvalidOperationException();

                client.Connection.WriteReply(FtpReplyCode.FileStatusOkay);

                client.DataThread = new Thread(() =>
                {
                    if (client.RestartPosition > 0)
                    {
                        stream.Position = client.RestartPosition;
                        client.RestartPosition = 0;
                    }
                    WriteToDataConnection(client, stream);
                });
                client.DataThread.Start();
            }
            else
                WriteBadSequence(client);
        }

        private void OnSize(Client client, string filename)
        {
            if (client.State == ClientState.LoggedIn)
            {
                var file = _fileSystemProvider.GetFile(filename);
                if (file == null)
                    throw new NotImplementedException();

                client.Connection.WriteReply(FtpReplyCode.FileStatus, file.GetSize().ToString());
            }
            else
                WriteBadSequence(client);
        }

        private void OnSyst(Client client, string param)
        {
            client.Connection.WriteReply(FtpReplyCode.SystemType, "win64");
        }

        private void OnType(Client client, string param)
        {
            client.Connection.WriteReply(FtpReplyCode.CommandOkay);
        }

        private void OnUser(Client client, string user)
        {
            if (client.State == ClientState.Greeting)
            {
                client.Connection.WriteReply(FtpReplyCode.UserNameOkayNeedPassword);
                client.State = ClientState.WaitPass;
            }
            else
                WriteBadSequence(client);
        }

        private void WriteBadSequence(Client client)
        {
            client.Connection.WriteReply(FtpReplyCode.BadSequence);
        }

        private void WriteToDataConnection(Client client, Stream fileStream)
        {
            try
            {
                using (var socket = client.PassiveListener.Accept())
                using (var networkStream = new NetworkStream(socket))
                    fileStream.CopyTo(networkStream);
            }
            catch (IOException ex)
            {
                if (Log != null)
                {
                    Log.WriteLine(ex.Message);
                    if (ex.InnerException != null)
                        Log.WriteLine(ex.InnerException.Message);
                }
                return;
            }
            finally
            {
                fileStream.Close();
            }

            CloseDataConnection(client);
        }

        private void CloseDataConnection(Client client)
        {
            client.Connection.WriteReply(FtpReplyCode.ClosingDataConnection);

            client.PassiveListener.Close();
            client.PassiveListener = null;

            lock (_occupiedPorts)
                _occupiedPorts.Remove(client.PassivePort);

            client.DataThread = null;
        }

        #endregion

        #region Nested classes

        private class Client
        {
            public Socket Socket;
            public FtpServerConnection Connection;
            public Thread Thread;
            public ClientState State;

            public Socket PassiveListener;
            public int PassivePort;
            public Thread DataThread;
            public long RestartPosition;
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