using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate;
using Zergatul.IO;
using Zergatul.Security.Tls;

namespace Zergatul.Security.OpenSsl
{
    class TlsStream : Tls.TlsStream
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
        private OpenSsl.VerifyCallbackDelegate _verifyCallback;

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
                    if (Parameters.BidirectionalShutdown && ret == 0)
                    {
                        /* The shutdown is not yet finished: the close_notify was sent but the peer did not send it back yet.
                         * Call SSL_read() to do a bidirectional shutdown.
                         * The output of SSL_get_error may be misleading, as an erroneous SSL_ERROR_SYSCALL may be flagged even though no error occurred.
                         */
                        throw new NotImplementedException();
                    }

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

            IntPtr method = _isServer ? OpenSsl.TLS_server_method() : OpenSsl.TLS_client_method();
            _sslContext = OpenSsl.SSL_CTX_new(method);
            if (_sslContext == IntPtr.Zero)
                throw new OpenSslException();

            _ssl = OpenSsl.SSL_new(_sslContext);
            if (_ssl == IntPtr.Zero)
                throw new OpenSslException();

            #region Version

            if (Parameters.MinVersion != null)
            {
                if (OpenSsl.SSL_set_min_proto_version(_ssl, TlsHelper.ToProtocolVersion(Parameters.MinVersion.Value)) != 1)
                    throw new OpenSslException();
            }

            if (Parameters.MaxVersion != null)
            {
                if (OpenSsl.SSL_set_max_proto_version(_ssl, TlsHelper.ToProtocolVersion(Parameters.MaxVersion.Value)) != 1)
                    throw new OpenSslException();
            }

            #endregion

            #region BIO

            OpenSsl.SSL_set_bio(_ssl, _wrapper.Bio, _wrapper.Bio);

            #endregion

            #region Certificate verification

            // store in field to disable gargage collecting of converted delegate
            _verifyCallback = CertificateValidateCallback;
            if (_isServer)
            {
                if (Parameters.RequestClientCertificate == true)
                    OpenSsl.SSL_set_verify(_ssl, OpenSsl.SSL_VERIFY_PEER, _verifyCallback);
                else
                    OpenSsl.SSL_set_verify(_ssl, OpenSsl.SSL_VERIFY_NONE, null);
            }
            else
            {
                OpenSsl.SSL_set_verify(_ssl, OpenSsl.SSL_VERIFY_PEER, _verifyCallback);
            }

            #endregion

            #region Ciphersuites

            if (Parameters.CipherSuites != null)
            {
                string cipherList = GetCipherList();
                if (OpenSsl.SSL_set_cipher_list(_ssl, cipherList) != 1)
                    throw new OpenSslException();

                if (Parameters.MaxVersion == null || Parameters.MaxVersion >= TlsVersion.Tls13)
                {
                    string cipherSuites = GetCipherSuites();
                    if (OpenSsl.SSL_set_ciphersuites(_ssl, cipherSuites) != 1)
                        throw new OpenSslException();
                }
            }

            #endregion
        }

        private int CertificateValidateCallback(int preverifyOk, IntPtr x509Ctx)
        {
            if (_isServer)
            {
                throw new NotImplementedException();
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

        private string GetCipherList()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Parameters.CipherSuites.Length; i++)
            {
                switch (Parameters.CipherSuites[i])
                {
                    // TLS v1.0 cipher suites
                    case CipherSuite.TLS_RSA_WITH_NULL_MD5: sb.Append("NULL-MD5"); break;
                    case CipherSuite.TLS_RSA_WITH_NULL_SHA: sb.Append("NULL-SHA"); break;
                    case CipherSuite.TLS_RSA_WITH_RC4_128_MD5: sb.Append("RC4-MD5"); break;
                    case CipherSuite.TLS_RSA_WITH_RC4_128_SHA: sb.Append("RC4-SHA"); break;
                    case CipherSuite.TLS_RSA_WITH_IDEA_CBC_SHA: sb.Append("IDEA-CBC-SHA"); break;
                    case CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA: sb.Append("DES-CBC3-SHA"); break;
                    case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA: sb.Append("DHE-DSS-DES-CBC3-SHA"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA: sb.Append("DHE-RSA-DES-CBC3-SHA"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_RC4_128_MD5: sb.Append("ADH-RC4-MD5"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_3DES_EDE_CBC_SHA: sb.Append("ADH-DES-CBC3-SHA"); break;

                    // AES ciphersuites from RFC3268, extending TLS v1.0
                    case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA: sb.Append("AES128-SHA"); break;
                    case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA: sb.Append("AES256-SHA"); break;
                    case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA: sb.Append("DH-DSS-AES128-SHA"); break;
                    case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA: sb.Append("DH-DSS-AES256-SHA"); break;
                    case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA: sb.Append("DH-RSA-AES128-SHA"); break;
                    case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA: sb.Append("DH-RSA-AES256-SHA"); break;
                    case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA: sb.Append("DHE-DSS-AES128-SHA"); break;
                    case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA: sb.Append("DHE-DSS-AES256-SHA"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA: sb.Append("DHE-RSA-AES128-SHA"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA: sb.Append("DHE-RSA-AES256-SHA"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_AES_128_CBC_SHA: sb.Append("ADH-AES128-SHA"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_AES_256_CBC_SHA: sb.Append("ADH-AES256-SHA"); break;

                    // Camellia ciphersuites from RFC4132, extending TLS v1.0
                    case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA: sb.Append("CAMELLIA128-SHA"); break;
                    case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA: sb.Append("CAMELLIA256-SHA"); break;
                    case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA: sb.Append("DH-DSS-CAMELLIA128-SHA"); break;
                    case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA: sb.Append("DH-DSS-CAMELLIA256-SHA"); break;
                    case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA: sb.Append("DH-RSA-CAMELLIA128-SHA"); break;
                    case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA: sb.Append("DH-RSA-CAMELLIA256-SHA"); break;
                    case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA: sb.Append("DHE-DSS-CAMELLIA128-SHA"); break;
                    case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA: sb.Append("DHE-DSS-CAMELLIA256-SHA"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA: sb.Append("DHE-RSA-CAMELLIA128-SHA"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA: sb.Append("DHE-RSA-CAMELLIA256-SHA"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_CAMELLIA_128_CBC_SHA: sb.Append("ADH-CAMELLIA128-SHA"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_CAMELLIA_256_CBC_SHA: sb.Append("ADH-CAMELLIA256-SHA"); break;

                    // SEED ciphersuites from RFC4162, extending TLS v1.0
                    case CipherSuite.TLS_RSA_WITH_SEED_CBC_SHA: sb.Append("SEED-SHA"); break;
                    case CipherSuite.TLS_DH_DSS_WITH_SEED_CBC_SHA: sb.Append("DH-DSS-SEED-SHA"); break;
                    case CipherSuite.TLS_DH_RSA_WITH_SEED_CBC_SHA: sb.Append("DH-RSA-SEED-SHA"); break;
                    case CipherSuite.TLS_DHE_DSS_WITH_SEED_CBC_SHA: sb.Append("DHE-DSS-SEED-SHA"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_SEED_CBC_SHA: sb.Append("DHE-RSA-SEED-SHA"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_SEED_CBC_SHA: sb.Append("ADH-SEED-SHA"); break;

                    // GOST ciphersuites from draft-chudov-cryptopro-cptls, extending TLS v1.0
                    case CipherSuite.TLS_GOSTR341094_WITH_28147_CNT_IMIT: sb.Append("GOST94-GOST89-GOST89"); break;
                    case CipherSuite.TLS_GOSTR341001_WITH_28147_CNT_IMIT: sb.Append("GOST2001-GOST89-GOST89"); break;
                    case CipherSuite.TLS_GOSTR341094_WITH_NULL_GOSTR3411: sb.Append("GOST94-NULL-GOST94"); break;
                    case CipherSuite.TLS_GOSTR341001_WITH_NULL_GOSTR3411: sb.Append("GOST2001-NULL-GOST94"); break;

                    // Additional Export 1024 and other cipher suites
                    //case CipherSuite.DHE_DSS_WITH_RC4_128_SHA: sb.Append("DHE-DSS-RC4-SHA"); break;

                    // Elliptic curve cipher suites
                    case CipherSuite.TLS_ECDHE_RSA_WITH_NULL_SHA: sb.Append("ECDHE-RSA-NULL-SHA"); break;
                    case CipherSuite.TLS_ECDHE_RSA_WITH_RC4_128_SHA: sb.Append("ECDHE-RSA-RC4-SHA"); break;
                    case CipherSuite.TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA: sb.Append("ECDHE-RSA-DES-CBC3-SHA"); break;
                    case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA: sb.Append("ECDHE-RSA-AES128-SHA"); break;
                    case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA: sb.Append("ECDHE-RSA-AES256-SHA"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_NULL_SHA: sb.Append("ECDHE-ECDSA-NULL-SHA"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_RC4_128_SHA: sb.Append("ECDHE-ECDSA-RC4-SHA"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA: sb.Append("ECDHE-ECDSA-DES-CBC3-SHA"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA: sb.Append("ECDHE-ECDSA-AES128-SHA"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA: sb.Append("ECDHE-ECDSA-AES256-SHA"); break;
                    case CipherSuite.TLS_ECDH_Anon_WITH_NULL_SHA: sb.Append("AECDH-NULL-SHA"); break;
                    case CipherSuite.TLS_ECDH_Anon_WITH_RC4_128_SHA: sb.Append("AECDH-RC4-SHA"); break;
                    case CipherSuite.TLS_ECDH_Anon_WITH_3DES_EDE_CBC_SHA: sb.Append("AECDH-DES-CBC3-SHA"); break;
                    case CipherSuite.TLS_ECDH_Anon_WITH_AES_128_CBC_SHA: sb.Append("AECDH-AES128-SHA"); break;
                    case CipherSuite.TLS_ECDH_Anon_WITH_AES_256_CBC_SHA: sb.Append("AECDH-AES256-SHA"); break;

                    // TLS v1.2 cipher suites
                    case CipherSuite.TLS_RSA_WITH_NULL_SHA256: sb.Append("NULL-SHA256"); break;
                    case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256: sb.Append("AES128-SHA256"); break;
                    case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256: sb.Append("AES256-SHA256"); break;
                    case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256: sb.Append("AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384: sb.Append("AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256: sb.Append("DH-RSA-AES128-SHA256"); break;
                    case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256: sb.Append("DH-RSA-AES256-SHA256"); break;
                    case CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256: sb.Append("DH-RSA-AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384: sb.Append("DH-RSA-AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256: sb.Append("DH-DSS-AES128-SHA256"); break;
                    case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256: sb.Append("DH-DSS-AES256-SHA256"); break;
                    case CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256: sb.Append("DH-DSS-AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384: sb.Append("DH-DSS-AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256: sb.Append("DHE-RSA-AES128-SHA256"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256: sb.Append("DHE-RSA-AES256-SHA256"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256: sb.Append("DHE-RSA-AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384: sb.Append("DHE-RSA-AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256: sb.Append("DHE-DSS-AES128-SHA256"); break;
                    case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256: sb.Append("DHE-DSS-AES256-SHA256"); break;
                    case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256: sb.Append("DHE-DSS-AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384: sb.Append("DHE-DSS-AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256: sb.Append("ECDHE-RSA-AES128-SHA256"); break;
                    case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384: sb.Append("ECDHE-RSA-AES256-SHA384"); break;
                    case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256: sb.Append("ECDHE-RSA-AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384: sb.Append("ECDHE-RSA-AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256: sb.Append("ECDHE-ECDSA-AES128-SHA256"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384: sb.Append("ECDHE-ECDSA-AES256-SHA384"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256: sb.Append("ECDHE-ECDSA-AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384: sb.Append("ECDHE-ECDSA-AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_AES_128_CBC_SHA256: sb.Append("ADH-AES128-SHA256"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_AES_256_CBC_SHA256: sb.Append("ADH-AES256-SHA256"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_AES_128_GCM_SHA256: sb.Append("ADH-AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_DH_Anon_WITH_AES_256_GCM_SHA384: sb.Append("ADH-AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_RSA_WITH_AES_128_CCM: sb.Append("AES128-CCM"); break;
                    case CipherSuite.TLS_RSA_WITH_AES_256_CCM: sb.Append("AES256-CCM"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM: sb.Append("DHE-RSA-AES128-CCM"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM: sb.Append("DHE-RSA-AES256-CCM"); break;
                    case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8: sb.Append("AES128-CCM8"); break;
                    case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8: sb.Append("AES256-CCM8"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8: sb.Append("DHE-RSA-AES128-CCM8"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8: sb.Append("DHE-RSA-AES256-CCM8"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM: sb.Append("ECDHE-ECDSA-AES128-CCM"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM: sb.Append("ECDHE-ECDSA-AES256-CCM"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8: sb.Append("ECDHE-ECDSA-AES128-CCM8"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8: sb.Append("ECDHE-ECDSA-AES256-CCM8"); break;

                    // Camellia HMAC-Based ciphersuites from RFC6367, extending TLS v1.2
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256: sb.Append("ECDHE-ECDSA-CAMELLIA128-SHA256"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384: sb.Append("ECDHE-ECDSA-CAMELLIA256-SHA384"); break;
                    case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256: sb.Append("ECDHE-RSA-CAMELLIA128-SHA256"); break;
                    case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384: sb.Append("ECDHE-RSA-CAMELLIA256-SHA384"); break;

                    // Pre-shared keying (PSK) ciphersuites
                    case CipherSuite.TLS_PSK_WITH_NULL_SHA: sb.Append("PSK-NULL-SHA"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA: sb.Append("DHE-PSK-NULL-SHA"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA: sb.Append("RSA-PSK-NULL-SHA"); break;
                    case CipherSuite.TLS_PSK_WITH_RC4_128_SHA: sb.Append("PSK-RC4-SHA"); break;
                    case CipherSuite.TLS_PSK_WITH_3DES_EDE_CBC_SHA: sb.Append("PSK-3DES-EDE-CBC-SHA"); break;
                    case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA: sb.Append("PSK-AES128-CBC-SHA"); break;
                    case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA: sb.Append("PSK-AES256-CBC-SHA"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA: sb.Append("DHE-PSK-RC4-SHA"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA: sb.Append("DHE-PSK-3DES-EDE-CBC-SHA"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA: sb.Append("DHE-PSK-AES128-CBC-SHA"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA: sb.Append("DHE-PSK-AES256-CBC-SHA"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_RC4_128_SHA: sb.Append("RSA-PSK-RC4-SHA"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA: sb.Append("RSA-PSK-3DES-EDE-CBC-SHA"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA: sb.Append("RSA-PSK-AES128-CBC-SHA"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA: sb.Append("RSA-PSK-AES256-CBC-SHA"); break;
                    case CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256: sb.Append("PSK-AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384: sb.Append("PSK-AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256: sb.Append("DHE-PSK-AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384: sb.Append("DHE-PSK-AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256: sb.Append("RSA-PSK-AES128-GCM-SHA256"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384: sb.Append("RSA-PSK-AES256-GCM-SHA384"); break;
                    case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA256: sb.Append("PSK-AES128-CBC-SHA256"); break;
                    case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA384: sb.Append("PSK-AES256-CBC-SHA384"); break;
                    case CipherSuite.TLS_PSK_WITH_NULL_SHA256: sb.Append("PSK-NULL-SHA256"); break;
                    case CipherSuite.TLS_PSK_WITH_NULL_SHA384: sb.Append("PSK-NULL-SHA384"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256: sb.Append("DHE-PSK-AES128-CBC-SHA256"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384: sb.Append("DHE-PSK-AES256-CBC-SHA384"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA256: sb.Append("DHE-PSK-NULL-SHA256"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA384: sb.Append("DHE-PSK-NULL-SHA384"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA256: sb.Append("RSA-PSK-AES128-CBC-SHA256"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384: sb.Append("RSA-PSK-AES256-CBC-SHA384"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA256: sb.Append("RSA-PSK-NULL-SHA256"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA384: sb.Append("RSA-PSK-NULL-SHA384"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_RC4_128_SHA: sb.Append("ECDHE-PSK-RC4-SHA"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_3DES_EDE_CBC_SHA: sb.Append("ECDHE-PSK-3DES-EDE-CBC-SHA"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA: sb.Append("ECDHE-PSK-AES128-CBC-SHA"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA: sb.Append("ECDHE-PSK-AES256-CBC-SHA"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256: sb.Append("ECDHE-PSK-AES128-CBC-SHA256"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384: sb.Append("ECDHE-PSK-AES256-CBC-SHA384"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA: sb.Append("ECDHE-PSK-NULL-SHA"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA256: sb.Append("ECDHE-PSK-NULL-SHA256"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA384: sb.Append("ECDHE-PSK-NULL-SHA384"); break;
                    case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_CBC_SHA256: sb.Append("PSK-CAMELLIA128-SHA256"); break;
                    case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_CBC_SHA384: sb.Append("PSK-CAMELLIA256-SHA384"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_CBC_SHA256: sb.Append("DHE-PSK-CAMELLIA128-SHA256"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384: sb.Append("DHE-PSK-CAMELLIA256-SHA384"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_CBC_SHA256: sb.Append("RSA-PSK-CAMELLIA128-SHA256"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_CBC_SHA384: sb.Append("RSA-PSK-CAMELLIA256-SHA384"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_128_CBC_SHA256: sb.Append("ECDHE-PSK-CAMELLIA128-SHA256"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384: sb.Append("ECDHE-PSK-CAMELLIA256-SHA384"); break;
                    case CipherSuite.TLS_PSK_WITH_AES_128_CCM: sb.Append("PSK-AES128-CCM"); break;
                    case CipherSuite.TLS_PSK_WITH_AES_256_CCM: sb.Append("PSK-AES256-CCM"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM: sb.Append("DHE-PSK-AES128-CCM"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM: sb.Append("DHE-PSK-AES256-CCM"); break;
                    case CipherSuite.TLS_PSK_WITH_AES_128_CCM_8: sb.Append("PSK-AES128-CCM8"); break;
                    case CipherSuite.TLS_PSK_WITH_AES_256_CCM_8: sb.Append("PSK-AES256-CCM8"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM_8: sb.Append("DHE-PSK-AES128-CCM8"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM_8: sb.Append("DHE-PSK-AES256-CCM8"); break;

                    // ChaCha20-Poly1305 cipher suites, extending TLS v1.2
                    case CipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256: sb.Append("ECDHE-RSA-CHACHA20-POLY1305"); break;
                    case CipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256: sb.Append("ECDHE-ECDSA-CHACHA20-POLY1305"); break;
                    case CipherSuite.TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256: sb.Append("DHE-RSA-CHACHA20-POLY1305"); break;
                    case CipherSuite.TLS_PSK_WITH_CHACHA20_POLY1305_SHA256: sb.Append("PSK-CHACHA20-POLY1305"); break;
                    case CipherSuite.TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256: sb.Append("ECDHE-PSK-CHACHA20-POLY1305"); break;
                    case CipherSuite.TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256: sb.Append("DHE-PSK-CHACHA20-POLY1305"); break;
                    case CipherSuite.TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256: sb.Append("RSA-PSK-CHACHA20-POLY1305"); break;

                    // Skip TLS v1.3
                    case CipherSuite.TLS_AES_128_GCM_SHA256:
                    case CipherSuite.TLS_AES_256_GCM_SHA384:
                    case CipherSuite.TLS_CHACHA20_POLY1305_SHA256:
                    case CipherSuite.TLS_AES_128_CCM_SHA256:
                    case CipherSuite.TLS_AES_128_CCM_8_SHA256:
                        break;

                    default:
                        throw new NotSupportedException();
                }

                if (sb.Length > 0 && sb[sb.Length - 1] != ':')
                    sb.Append(':');
            }

            if (sb[sb.Length - 1] == ':')
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private string GetCipherSuites()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Parameters.CipherSuites.Length; i++)
            {
                switch (Parameters.CipherSuites[i])
                {
                    case CipherSuite.TLS_AES_128_GCM_SHA256: sb.Append("TLS_AES_128_GCM_SHA256"); break;
                    case CipherSuite.TLS_AES_256_GCM_SHA384: sb.Append("TLS_AES_256_GCM_SHA384"); break;
                    case CipherSuite.TLS_CHACHA20_POLY1305_SHA256: sb.Append("TLS_CHACHA20_POLY1305_SHA256"); break;
                    case CipherSuite.TLS_AES_128_CCM_SHA256: sb.Append("TLS_AES_128_CCM_SHA256"); break;
                    case CipherSuite.TLS_AES_128_CCM_8_SHA256: sb.Append("TLS_AES_128_CCM_8_SHA256"); break;
                }

                if (sb.Length > 0 && sb[sb.Length - 1] != ':')
                    sb.Append(':');
            }

            if (sb[sb.Length - 1] == ':')
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
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