using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate;
using Zergatul.IO;

namespace Zergatul.Security.OpenSsl
{
    class TlsStream : Zergatul.Security.TlsStream
    {
        private Stream _stream;
        private BioStreamWrapper _wrapper;
        private State _state;
        private byte[] _writeBuffer;
        private GCHandle _writeBufferHandle;
        private int _writeBufferPos;

        private IntPtr _sslContext;
        private IntPtr _ssl;

        private bool _isServer;
        private OpenSsl.VerifyCallbackDelegate VerifyCallback;

        public TlsStream(Stream innerStream)
        {
            _stream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            _wrapper = new BioStreamWrapper(_stream);
            _state = State.Init;
        }

        public override void AuthenticateAsClient()
        {
            _isServer = false;
            Init();
            int ret = OpenSsl.SSL_connect(_ssl);
            if (ret != 1)
            {
                ProcessError(ret);
                return;
            }
            OnAuthFinished();
            _state = State.Authenticated;
        }

        public override Task AuthenticateAsClientAsync()
        {
            _isServer = false;
            Init();
            throw new NotImplementedException();
        }

        public override void AuthenticateAsServer()
        {
            _isServer = true;
            Init();
            int ret = OpenSsl.SSL_accept(_ssl);
            if (ret != 1)
            {
                ProcessError(ret);
                return;
            }
            OnAuthFinished();
            _state = State.Authenticated;
        }

        public override Task AuthenticateAsServerAsync()
        {
            _isServer = true;
            Init();
            throw new NotImplementedException();
        }

        #region Stream overrides

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_stream != null)
                {
                    int ret = OpenSsl.SSL_shutdown(_ssl);
                    if (ret < 0)
                        ProcessError(ret);

                    if (!Parameters.LeaveOpen)
                        _stream.Dispose();
                    _stream = null;
                }
            }

            if (_writeBuffer != null)
            {
                _writeBuffer = null;
                _writeBufferHandle.Free();
            }

            if (_wrapper != null)
            {
                _wrapper.SetFree();
                _wrapper.Dispose();
                _wrapper = null;
            }

            if (_ssl != IntPtr.Zero)
            {
                OpenSsl.SSL_free(_ssl);
                _ssl = IntPtr.Zero;
            }

            if (_sslContext != IntPtr.Zero)
            {
                OpenSsl.SSL_CTX_free(_sslContext);
                _sslContext = IntPtr.Zero;
            }
        }

        public override void Flush()
        {
            if (_state != State.Authenticated)
                throw new InvalidOperationException();

            if (_writeBufferPos != 0)
            {
                int count = _writeBufferPos;
                _writeBufferPos = 0;
                int ret = OpenSsl.SSL_write(_ssl, _writeBufferHandle.AddrOfPinnedObject(), count);
                _writeBufferPos = 0;
                if (ret <= 0)
                    ProcessError(ret);
                if (ret != count)
                    throw new NotImplementedException();
                _wrapper.Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_state != State.Authenticated)
                throw new InvalidOperationException();
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                int ret = OpenSsl.SSL_read(_ssl, handle.AddrOfPinnedObject() + offset, count);
                if (ret <= 0)
                    ProcessError(ret);
                return ret;
            }
            finally
            {
                handle.Free();
            }
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_state != State.Authenticated)
                throw new InvalidOperationException();
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return;

            while (count > 0)
            {
                int copy = System.Math.Min(_writeBuffer.Length - _writeBufferPos, count);
                Buffer.BlockCopy(buffer, offset, _writeBuffer, _writeBufferPos, copy);
                _writeBufferPos += copy;
                offset += copy;
                count -= copy;

                if (_writeBufferPos == _writeBuffer.Length)
                {
                    int ret = OpenSsl.SSL_write(_ssl, _writeBufferHandle.AddrOfPinnedObject(), _writeBuffer.Length);
                    _writeBufferPos = 0;
                    if (ret <= 0)
                        ProcessError(ret);
                    if (ret != _writeBuffer.Length)
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        #region Private methods

        private void Init()
        {
            if (_state != State.Init)
                throw new InvalidOperationException();

            IntPtr method;
            switch (Parameters.Version ?? TlsVersion.Tls12)
            {
                case TlsVersion.Tls10:
                    method = _isServer ? OpenSsl.TLSv1_server_method() : OpenSsl.TLSv1_client_method();
                    break;

                case TlsVersion.Tls11:
                    method = _isServer ? OpenSsl.TLSv1_1_server_method() : OpenSsl.TLSv1_1_client_method();
                    break;

                case TlsVersion.Tls12:
                    method = _isServer ? OpenSsl.TLSv1_2_server_method() : OpenSsl.TLSv1_2_client_method();
                    break;

                default:
                    throw new NotSupportedException();
            }
            if (method == IntPtr.Zero)
                throw new OpenSslException();

            _sslContext = OpenSsl.SSL_CTX_new(method);
            if (_sslContext == IntPtr.Zero)
                throw new OpenSslException();

            _ssl = OpenSsl.SSL_new(_sslContext);
            if (_ssl == IntPtr.Zero)
                throw new OpenSslException();

            // store in field to disable gargage collecting of converted delegate
            VerifyCallback = CertificateValidateCallback;
            if (_isServer)
            {
                if (Parameters.RequestClientCertificate == true)
                    OpenSsl.SSL_set_verify(_ssl, OpenSsl.SSL_VERIFY_PEER, VerifyCallback);
                else
                    OpenSsl.SSL_set_verify(_ssl, OpenSsl.SSL_VERIFY_NONE, null);
            }
            else
            {
                OpenSsl.SSL_set_verify(_ssl, OpenSsl.SSL_VERIFY_PEER, VerifyCallback);
            }

            OpenSsl.SSL_set_bio(_ssl, _wrapper.Bio, _wrapper.Bio);
        }

        private int CertificateValidateCallback(int preverifyOk, IntPtr x509Ctx)
        {
            if (_isServer)
            {
                return 0;
            }
            else
            {
                var validate = Parameters.ServerCertificateValidateCallback;
                if (validate != null)
                {
                    IntPtr x509 = OpenSsl.X509_STORE_CTX_get_current_cert(x509Ctx);
                    if (x509 == IntPtr.Zero)
                        throw new InvalidOperationException();
                    IntPtr ptr = IntPtr.Zero;
                    int length = OpenSsl.i2d_X509(x509, ref ptr);
                    if (length < 0)
                        throw new OpenSslException();
                    byte[] raw = new byte[length];
                    var handle = GCHandle.Alloc(raw, GCHandleType.Pinned);
                    try
                    {
                        ptr = handle.AddrOfPinnedObject();
                        if (OpenSsl.i2d_X509(x509, ref ptr) != length)
                            throw new InvalidOperationException();
                    }
                    finally
                    {
                        handle.Free();
                    }
                    var certificate = new X509Certificate(raw);
                    return validate(certificate) ? 1 : 0;
                }
                else
                {
                    return 0;
                }
            }
        }

        private void OnAuthFinished()
        {
            if (_writeBuffer == null)
            {
                _writeBuffer = new byte[Network.Tls.Messages.RecordMessageStream.PlaintextLimit];
                _writeBufferHandle = GCHandle.Alloc(_writeBuffer, GCHandleType.Pinned);
                _writeBufferPos = 0;
            }
        }

        private void ProcessError(int ret)
        {
            switch (OpenSsl.SSL_get_error(_ssl, ret))
            {
                case OpenSsl.SSL_ERROR_NONE:
                    return;

                case OpenSsl.SSL_ERROR_ZERO_RETURN:
                    // The TLS/SSL peer has closed the connection for writing by sending the "close notify" alert
                    _state = State.Closed;
                    return;

                case OpenSsl.SSL_ERROR_WANT_READ:
                    throw new NotImplementedException();

                case OpenSsl.SSL_ERROR_WANT_WRITE:
                    throw new NotImplementedException();

                case OpenSsl.SSL_ERROR_WANT_ACCEPT:
                case OpenSsl.SSL_ERROR_WANT_CONNECT:
                    throw new NotImplementedException();

                case OpenSsl.SSL_ERROR_WANT_X509_LOOKUP:
                    throw new OpenSslException("The operation did not complete because an application callback set by SSL_CTX_set_client_cert_cb() has asked to be called again");

                case OpenSsl.SSL_ERROR_WANT_ASYNC:
                    throw new OpenSslException("The operation did not complete because an asynchronous engine is still processing data");

                case OpenSsl.SSL_ERROR_WANT_ASYNC_JOB:
                    throw new OpenSslException("The asynchronous job could not be started because there were no async jobs available in the pool");

                case OpenSsl.SSL_ERROR_WANT_CLIENT_HELLO_CB:
                    throw new OpenSslException("The operation did not complete because an application callback set by SSL_CTX_set_client_hello_cb() has asked to be called again");

                case OpenSsl.SSL_ERROR_SYSCALL:
                    // Some non-recoverable I/O error occurred
                    // The OpenSSL error queue may contain more information on the error
                    throw new OpenSslException();

                case OpenSsl.SSL_ERROR_SSL:
                    // A failure in the SSL library occurred, usually a protocol error
                    // The OpenSSL error queue contains more information on the error
                    throw new OpenSslException();
            }
        }

        #endregion

        #region Nested Classes

        private enum State
        {
            Init,
            Authenticated,
            Closed
        }

        #endregion
    }
}