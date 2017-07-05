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
using Zergatul.Network;
using Zergatul.Network.Proxy;

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

    // TODO: ConnectTimeout with proxy

    public class FtpConnection : IDisposable
    {
        #region private static

        private static readonly Regex DataPortRegex = new Regex(@"\((\d+),(\d+),(\d+),(\d+),(\d+),(\d+)\)");
        private static readonly Regex ExPassiveModeRegex = new Regex(@"\(\|\|\|(\d+)\|\)");

        #endregion

        #region private instance

        private TcpClient _tcpClient;
        private string _host;
        private IPAddress _address;
        private Stream _commandStream, _baseCommandStream;
        private FtpControlStreamReader _controlStreamReader;
        private Stream CommandStream
        {
            set
            {
                _commandStream = value;
                _controlStreamReader = new FtpControlStreamReader(value);
            }
            get
            {
                return _commandStream;
            }
        }
        private byte[] _readBuffer, _passiveBuffer;
        private List<byte> _messageBytes;
        private bool _tls, _dataChannelTls;
        private bool? _passive;
        private IPEndPoint _dataConnectionIPEndPoint;
        private IPAddress _resolvedHost;

        private bool _asyncOperationInProcess;

        #endregion

        #region public properties

        /// <summary>
        /// All comunication via control connection will be logged here
        /// </summary>
        public TextWriter Log { get; set; }

        /// <summary>
        /// Client certificates for secure connection
        /// </summary>
        public X509CertificateCollection X509CertificateCollection { get; set; }

        /// <summary>
        /// Overrides default cerification validation
        /// </summary>
        public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }

        /// <summary>
        /// When connecting by host name, which IP address should be preffered
        /// Default: true
        /// </summary>
        public bool PreferIPv4 { get; set; }

        /// <summary>
        /// Proxy for both control and data channels
        /// </summary>
        public ProxyBase Proxy { get; set; }

        /// <summary>
        /// When false, error responces will be stored in LastError property
        /// Default: true
        /// </summary>
        public bool ThrowExceptionOnServerErrors { get; set; }

        /// <summary>
        /// Validate server reply codes accordingly to RFC specifications
        /// If false, the validation only take into account first digit from 3-digit code
        /// Default: true
        /// </summary>
        public bool StrictMode { get; set; }

        /// <summary>
        /// When ThrowExceptionOnServerErrors is false, error server rely will be stored here
        /// </summary>
        public FtpServerReply LastError { get; private set; }

        /// <summary>
        /// Connection timeout in milliseconds, after which exception will be thrown
        /// Default: 15000
        /// </summary>
        public int ConnectTimeout { get; set; }

        /// <summary>
        /// When you connect to FTP server by using host name, this host name will be used for vertificate validation.
        /// If you connect by using IP address, you need to specify this property for certificate validation.
        /// You may use it for any other reason, if you connect by using one host name, but need validation using other host name.
        /// </summary>
        public string OverrideHostNameForCertificateValidation { get; set; }

        public Encoding ControlConnectionEncoding { get; set; }
        public Encoding DataConnectionEncoding { get; set; }

        public string Greeting { get; private set; }
        public string OperatingSystem { get; private set; }
        public FtpTransferMode TransferMode { get; private set; }

        #endregion

        #region ctor + connection establishment

        public FtpConnection()
        {
            this._readBuffer = new byte[256];
            this._passiveBuffer = new byte[1024];
            this._messageBytes = new List<byte>(256);
            this.X509CertificateCollection = new X509CertificateCollection();
            this.ThrowExceptionOnServerErrors = true;
            this.PreferIPv4 = true;
            this.StrictMode = true;
            this.ConnectTimeout = 15000;
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
                        if (!_tcpClient.ConnectAsync(this._address, port).Wait(ConnectTimeout))
                            throw new FtpException(string.Format("Connection timeout failed to {0} (using IP {1}).", host, this._address));
                    }
                }
                else
                {
                    this._address = ipHE.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetworkV6);
                    if (this._address != null)
                    {
                        _tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
                        if (!_tcpClient.ConnectAsync(this._address, port).Wait(ConnectTimeout))
                            throw new FtpException(string.Format("Connection timeout failed to {0} (using IP {1}).", host, this._address));
                    }
                }
                if (this._address == null)
                {
                    this._address = ipHE.AddressList.First();
                    _tcpClient = new TcpClient(this._address.AddressFamily);
                    if (!_tcpClient.ConnectAsync(this._address, port).Wait(ConnectTimeout))
                        throw new FtpException(string.Format("Connection timeout failed to {0} (using IP {1}).", host, this._address));
                }
            }
            if (!_tcpClient.Connected)
                throw new Exception("Unknown error");
            CommandStream = _tcpClient.GetStream();
            Greeting = _controlStreamReader.ReadServerReply().Message;
            if (Log != null)
                Log.WriteLine(Greeting);
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
                if (!_tcpClient.ConnectAsync(address, port).Wait(ConnectTimeout))
                    throw new FtpException(string.Format("Connection timeout failed to {0}.", _address));
            }
            if (!_tcpClient.Connected)
                throw new Exception("Unknown error");
            CommandStream = _tcpClient.GetStream();
            Greeting = _controlStreamReader.ReadServerReply().Message;
            TransferMode = FtpTransferMode.Stream;
        }

        #endregion

        #region Access control

        public void Login(string user, string password)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply;
            reply = SendCommand(FtpCommands.USER, user);
            CheckReply(reply);
            if (reply.Code == FtpReplyCode.UserLoggedIn)
                return;

            reply = SendCommand(FtpCommands.PASS, password);
            CheckReply(reply);
        }

        public void Account(string acct)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.ACCT);
            CheckReply(reply);
        }

        public void Reinitialize()
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.REIN);
            CheckReply(reply);

            if (_tls)
            {
                CommandStream.Dispose();
                CommandStream = _baseCommandStream;
            }
        }

        public void Quit()
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.QUIT);
            CheckReply(reply);

            _tcpClient.Close();
        }

        #region RFC2228

        public void AuthTls()
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.AUTH, "TLS");
            CheckReply(reply);

            _baseCommandStream = CommandStream;
            CommandStream = AuthenticateStreamWithTls(CommandStream);
            _tls = true;
            _dataChannelTls = false;
        }

        public void ProtectionBufferSize(int size)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.PBSZ, size.ToString());
            CheckReply(reply);
        }

        public void DataChannelProtectionLevel(FtpDataChannelProtectionLevel level)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.PROT, level.ToCommand());
            CheckReply(reply);

            _dataChannelTls = level != FtpDataChannelProtectionLevel.Clear;
        }

        public void ClearCommandChannel()
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.CCC);
            CheckReply(reply);

            // dispose SslStream; this will remain underlying NetworkStream opened
            CommandStream.Dispose();
            CommandStream = _baseCommandStream;
        }

        #endregion

        // SMNT

        #endregion

        #region Current directory

        public void ChangeWorkingDirectory(string dir)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.CWD, dir);
            CheckReply(reply);
        }

        public void ChangeToParentDirectory()
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.CDUP);
            CheckReply(reply);
        }

        // PWD

        #endregion

        #region Transfer parameter

        public void EnterPassiveMode()
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.PASV);
            CheckReply(reply);

            Match m = DataPortRegex.Match(reply.Message);
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
            _dataConnectionIPEndPoint = new IPEndPoint(ip, port);
            _passive = true;
        }

        public void EnterPassiveModeEx()
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.EPSV);
            CheckReply(reply);
            var m = ExPassiveModeRegex.Match(reply.Message);
            if (!m.Success)
                throw new Exception("parse fail");

            int port = int.Parse(m.Groups[1].Value);
            if (string.IsNullOrEmpty(_host))
                _dataConnectionIPEndPoint = new IPEndPoint(_address, port);
            else
            {
                var ipv4 = Dns.GetHostEntry(_host).AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                if (ipv4 != null)
                    _dataConnectionIPEndPoint = new IPEndPoint(ipv4, port);
                else
                    _dataConnectionIPEndPoint = new IPEndPoint(Dns.GetHostEntry(_host).AddressList[0], port);
            }
            _passive = true;
        }

        public void EnterActiveMode(IPAddress address, int port)
        {
            if (address.AddressFamily != AddressFamily.InterNetwork)
                throw new FtpException("Only IPv4 addresses are supported. Use EnterActiveModeEx for IPv6.");

            CheckStateBeforeCommand();

            string parameter = string.Join(",", address.GetAddressBytes().Concat(new byte[] { (byte)(port / 256), (byte)(port % 256) }).Select(b => b.ToString()));
            FtpServerReply reply = SendCommand(FtpCommands.PORT, parameter);
            CheckReply(reply);

            _dataConnectionIPEndPoint = new IPEndPoint(address, port);
            _passive = false;
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
            FtpServerReply reply = SendCommand(FtpCommands.EPRT, parameter);
            CheckReply(reply);

            _dataConnectionIPEndPoint = new IPEndPoint(address, port);
            _passive = false;
        }

        public void SetTransferMode(FtpTransferMode mode)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.MODE, mode.ToCommand());
            CheckReply(reply);

            TransferMode = mode;
        }

        public void SetRepresentationType(string type, string parameter)
        {
            CheckStateBeforeCommand();

            string p = type;
            if (!string.IsNullOrEmpty(parameter))
                p += " " + parameter;

            FtpServerReply reply = SendCommand(FtpCommands.TYPE, p);
            CheckReply(reply);
        }

        // STRU

        #endregion

        #region Directory/Files commands

        #region Retrieve File

        public void RetrieveFile(string file, Stream destination)
        {
            RetrieveFile(file, 0, destination);
        }

        public void RetrieveFile(string file, int from, Stream destination)
        {
            CheckStateBeforeCommand();

            var stream = CreateDataConnection();

            FtpServerReply reply;
            if (from != 0)
            {
                reply = SendCommand(FtpCommands.REST, from.ToString());
                CheckReply(reply);
            }
            reply = SendCommand(FtpCommands.RETR, file);
            CheckReply(reply);

            new FtpDataStreamReader(stream, TransferMode).Read(destination);
            stream.Close();

            ReadReplyAfterDataTransfer();
        }

        public Task RetrieveFileAsync(string file, Stream destination, CancellationToken cancellationToken = default(CancellationToken), IProgress<long> progress = null)
        {
            return RetrieveFileAsync(file, 0, destination, cancellationToken, progress);
        }

        public Task RetrieveFileAsync(string file, int from, Stream destination, CancellationToken cancellationToken = default(CancellationToken), IProgress<long> progress = null)
        {
            return Task.Run(() =>
                {
                    CheckStateBeforeCommand();

                    this._asyncOperationInProcess = true;
                    bool dataConnectionOpened = false;
                    Stream stream = null;
                    try
                    {
                        stream = CreateDataConnection();
                        dataConnectionOpened = true;

                        FtpServerReply reply;
                        if (from != 0)
                        {
                            reply = SendCommand(FtpCommands.REST, from.ToString());
                            CheckReply(reply);
                        }
                        reply = SendCommand(FtpCommands.RETR, file);
                        CheckReply(reply);

                        new FtpDataStreamReader(stream, TransferMode).ReadAsync(destination, cancellationToken, progress);
                        stream.Close();

                        ReadReplyAfterDataTransfer();
                    }
                    catch (OperationCanceledException)
                    {
                        if (dataConnectionOpened)
                        {
                            FtpServerReply reply = SendCommand(FtpCommands.ABOR);
                            stream.Close();
                            ReadReplyAfterDataTransfer();
                        }
                        throw;
                    }
                    finally
                    {
                        this._asyncOperationInProcess = false;
                    }
                }, cancellationToken);
        }

        #endregion

        #region Store File

        public void StoreFile(string file, Stream content)
        {
            StoreFile(file, 0, content);
        }

        public void StoreFile(string file, int from, Stream content)
        {
            CheckStateBeforeCommand();

            var stream = CreateDataConnection();

            FtpServerReply reply;
            if (from != 0)
            {
                reply = SendCommand(FtpCommands.REST, from.ToString());
                CheckReply(reply);
            }
            reply = SendCommand(FtpCommands.STOR, file);
            CheckReply(reply);

            new FtpDataStreamWriter(stream, TransferMode).Write(content);
            stream.Close();

            ReadReplyAfterDataTransfer();
        }

        public Task StoreFileAsync(string file, Stream content, CancellationToken cancellationToken = default(CancellationToken), IProgress<long> progress = null)
        {
            return StoreFileAsync(file, 0, content, cancellationToken, progress);
        }

        public Task StoreFileAsync(string file, int from, Stream content, CancellationToken cancellationToken = default(CancellationToken), IProgress<long> progress = null)
        {
            return Task.Run(() =>
            {
                CheckStateBeforeCommand();

                this._asyncOperationInProcess = true;
                bool dataConnectionOpened = false;
                Stream stream = null;
                try
                {
                    stream = CreateDataConnection();
                    dataConnectionOpened = true;

                    FtpServerReply reply;
                    if (from != 0)
                    {
                        reply = SendCommand(FtpCommands.REST, from.ToString());
                        CheckReply(reply);
                    }
                    reply = SendCommand(FtpCommands.STOR, file);
                    CheckReply(reply);

                    new FtpDataStreamWriter(stream, TransferMode).WriteAsync(content, cancellationToken, progress);
                    stream.Close();

                    ReadReplyAfterDataTransfer();
                }
                catch (OperationCanceledException)
                {
                    if (dataConnectionOpened)
                    {
                        FtpServerReply reply = SendCommand(FtpCommands.ABOR);
                        stream.Close();
                        ReadReplyAfterDataTransfer();
                    }
                    throw;
                }
                finally
                {
                    this._asyncOperationInProcess = false;
                }
            }, cancellationToken);
        }

        #endregion

        #region Append File

        public void AppendFile(string file, Stream content)
        {
            CheckStateBeforeCommand();

            var stream = CreateDataConnection();

            FtpServerReply reply = SendCommand(FtpCommands.APPE, file);
            CheckReply(reply);

            new FtpDataStreamWriter(stream, TransferMode).Write(content);
            stream.Close();

            ReadReplyAfterDataTransfer();
        }

        public Task AppendFileAsync(string file, Stream content, CancellationToken cancellationToken = default(CancellationToken), IProgress<long> progress = null)
        {
            return Task.Run(() =>
            {
                CheckStateBeforeCommand();

                this._asyncOperationInProcess = true;
                bool dataConnectionOpened = false;
                Stream stream = null;
                try
                {
                    stream = CreateDataConnection();
                    dataConnectionOpened = true;

                    FtpServerReply reply;
                    reply = SendCommand(FtpCommands.APPE, file);
                    CheckReply(reply);

                    new FtpDataStreamWriter(stream, TransferMode).WriteAsync(content, cancellationToken, progress);
                    stream.Close();

                    ReadReplyAfterDataTransfer();
                }
                catch (OperationCanceledException)
                {
                    if (dataConnectionOpened)
                    {
                        FtpServerReply reply = SendCommand(FtpCommands.ABOR);
                        stream.Close();
                        ReadReplyAfterDataTransfer();
                    }
                    throw;
                }
                finally
                {
                    this._asyncOperationInProcess = false;
                }
            }, cancellationToken);
        }

        #endregion

        public string List(string path = null)
        {
            CheckStateBeforeCommand();

            var stream = CreateDataConnection();

            FtpServerReply reply = SendCommand(FtpCommands.LIST, path);
            CheckReply(reply);

            var ms = new MemoryStream();
            new FtpDataStreamReader(stream, TransferMode).Read(ms);
            stream.Close();

            ReadReplyAfterDataTransfer();

            return Encoding.ASCII.GetString(ms.ToArray());
        }

        public void NameList(string path = null)
        {
            CheckStateBeforeCommand();

            var stream = CreateDataConnection();

            FtpServerReply reply = SendCommand(FtpCommands.NLST, path);
            CheckReply(reply);

            var ms = new MemoryStream();
            new FtpDataStreamReader(stream, TransferMode).Read(ms);
            stream.Close();

            ReadReplyAfterDataTransfer();

            if (Log != null)
                Log.WriteLine(Encoding.ASCII.GetString(ms.ToArray()));
        }

        public void DeleteFile(string file)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.DELE, file);
            CheckReply(reply);
        }

        public void RenameFile(string oldName, string newName)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply;
            reply = SendCommand(FtpCommands.RNFR, oldName);
            CheckReply(reply);
            reply = SendCommand(FtpCommands.RNTO, newName);
            CheckReply(reply);
        }

        public void MakeDirectory(string dir)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.MKD, dir);
            CheckReply(reply);
        }

        public void RemoveDirectory(string dir)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.RMD, dir);
            CheckReply(reply);
        }

        #region RFC 3659

        public void MachineListingSingle(string path = null)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.MLST, path);
            CheckReply(reply);
        }

        public void MachineListingMany(string path = null)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.MLSD, path);
            CheckReply(reply);
        }

        #endregion

        // STOU, ALLO

        #endregion

        #region Other commands

        public void System()
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.SYST);
            CheckReply(reply);

            OperatingSystem = reply.Message;
        }

        public void Help(string cmd)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.HELP, cmd);
            CheckReply(reply);
        }

        public string Features()
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.FEAT);
            CheckReply(reply);

            return reply.Message;
        }

        public string Site(string command)
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.SITE, command);
            CheckReply(reply);

            return reply.Message;
        }

        public void Noop()
        {
            CheckStateBeforeCommand();

            FtpServerReply reply = SendCommand(FtpCommands.NOOP);
            CheckReply(reply);
        }

        // STAT

        #endregion

        #region Private helper methods

        private void CheckStateBeforeCommand()
        {
            if (this._tcpClient == null)
                throw new InvalidOperationException("No connection. Use Connect method.");
            if (this._asyncOperationInProcess)
                throw new InvalidOperationException("Async operation currently in process.");
        }

        private void CheckReply(FtpServerReply reply)
        {
            if ((int)reply.Code >= 500)
            {
                if (ThrowExceptionOnServerErrors)
                    throw new FtpServerException(reply.Message, reply.Code);
            }
        }

        private void ReadReplyAfterDataTransfer()
        {
            FtpServerReply reply = _controlStreamReader.ReadServerReply();
            if (Log != null)
                Log.WriteLine(reply.Message);
            CheckReply(reply);
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
            string host = this._host ?? this.OverrideHostNameForCertificateValidation;
            if (host == null)
                throw new FtpException("Cannot validate certificate, because you connected using IP address. Please specify OverrideHostNameForCertificateValidation property.");
            stream.AuthenticateAsClient(
                host,
                this.X509CertificateCollection,
                SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
                true);
            return stream;
        }

        private Stream CreateDataConnection()
        {
            if (!_passive == null)
                throw new FtpException("You must enter active or passive mode to open data connections.");
            if (_passive.Value)
            {
                TcpClient passiveConnection;
                if (Proxy != null)
                    passiveConnection = Proxy.CreateConnection(_dataConnectionIPEndPoint.Address, _dataConnectionIPEndPoint.Port);
                else
                {
                    passiveConnection = new TcpClient(_dataConnectionIPEndPoint.Address.AddressFamily);
                    if (!passiveConnection.ConnectAsync(_dataConnectionIPEndPoint.Address, _dataConnectionIPEndPoint.Port).Wait(ConnectTimeout))
                        throw new FtpException("Data connection timeout failed");
                }

                Stream result = passiveConnection.GetStream();

                if (_tls && _dataChannelTls)
                    result = AuthenticateStreamWithTls(result);

                return result;
            }
            else
            {
                TcpListener activeConnectionListener;
                if (Proxy != null)
                    throw new NotImplementedException("proxy with active mode not implemented");

                activeConnectionListener = new TcpListener(_dataConnectionIPEndPoint);
                activeConnectionListener.Start();
                var task = activeConnectionListener.AcceptTcpClientAsync();
                if (!task.Wait(ConnectTimeout))
                    throw new FtpException("Data connection timeout failed");
                TcpClient activeConnection = task.Result;
                activeConnectionListener.Stop();

                Stream result = activeConnection.GetStream();

                if (_tls && _dataChannelTls)
                    result = AuthenticateStreamWithTls(result);

                return result;
            }
        }

        private FtpServerReply SendCommand(string command, string param = null)
        {
            string commandWithParam = command + (string.IsNullOrEmpty(param) ? "" : " " + param);
            var bytes = Encoding.ASCII.GetBytes(commandWithParam  + Constants.TelnetEndOfLine);
            if (Log != null)
                Log.WriteLine(commandWithParam);
            CommandStream.Write(bytes, 0, bytes.Length);
            FtpServerReply reply = _controlStreamReader.ReadServerReply();
            if (Log != null)
                Log.WriteLine(reply.Message);
            return reply;
        }

        #endregion

        #region Dispose pattern

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_tcpClient != null && _tcpClient.Connected)
                {
                    Quit();
                    _tcpClient.Close();
                }
            }
        }

        #endregion
    }
}