using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Zergatul.IO;

namespace Zergatul.Security.OpenSsl
{
    class TlsStream : Zergatul.Security.TlsStream
    {
        private Stream _stream;
        private BioStreamWrapper _wrapper;
        private State _state;

        private IntPtr _sslContext;
        private IntPtr _ssl;

        public TlsStream(Stream innerStream)
        {
            _stream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            _wrapper = new BioStreamWrapper(_stream);
            _state = State.Init;
        }

        public override void AuthenticateAsClient()
        {
            Init(server: false);
            int ret = OpenSsl.SSL_connect(_ssl);
            if (ret != 1)
                ProcessError(ret);
            _state = State.Authenticated;
        }

        public override Task AuthenticateAsClientAsync()
        {
            Init(server: false);
            throw new NotImplementedException();
        }

        public override void AuthenticateAsServer()
        {
            Init(server: true);
            int ret = OpenSsl.SSL_accept(_ssl);
            if (ret != 1)
                ProcessError(ret);
            _state = State.Authenticated;
        }

        public override Task AuthenticateAsServerAsync()
        {
            Init(server: true);
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

            _wrapper?.Dispose();
            _wrapper = null;

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
            throw new NotImplementedException();
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

            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                int ret = OpenSsl.SSL_write(_ssl, handle.AddrOfPinnedObject() + offset, count);
                if (ret <= 0)
                    ProcessError(ret);
                if (ret != count)
                    throw new NotImplementedException();
            }
            finally
            {
                handle.Free();
            }
        }

        #endregion

        #region Private methods

        private void Init(bool server)
        {
            IntPtr method;
            switch (Parameters.Version ?? TlsVersion.Tls12)
            {
                case TlsVersion.Tls10:
                    method = server ? OpenSsl.TLSv1_server_method() : OpenSsl.TLSv1_client_method();
                    break;

                case TlsVersion.Tls11:
                    method = server ? OpenSsl.TLSv1_1_server_method() : OpenSsl.TLSv1_1_client_method();
                    break;

                case TlsVersion.Tls12:
                    method = server ? OpenSsl.TLSv1_2_server_method() : OpenSsl.TLSv1_2_client_method();
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

            OpenSsl.SSL_set0_rbio(_ssl, _wrapper.Bio);
            OpenSsl.SSL_set0_wbio(_ssl, _wrapper.Bio);
        }

        private void ProcessError(int ret)
        {
            switch (OpenSsl.SSL_get_error(_ssl, ret))
            {
                case OpenSsl.SSL_ERROR_NONE:
                    return;

                case OpenSsl.SSL_ERROR_ZERO_RETURN:
                    throw new OpenSslException("The TLS/SSL peer has closed the connection for writing by sending the \"close notify\" alert");

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
            Authenticated
        }

        #endregion
    }
}