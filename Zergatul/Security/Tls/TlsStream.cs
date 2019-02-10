using System;
using System.IO;
using System.Threading.Tasks;

namespace Zergatul.Security.Tls
{
    public abstract class TlsStream : Stream
    {
        private TlsStreamParameters _parameters;
        public virtual TlsStreamParameters Parameters
        {
            get => _parameters;
            set => _parameters = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TlsStream()
        {
            _parameters = new TlsStreamParameters();
        }

        public abstract void AuthenticateAsClient();
        public abstract Task AuthenticateAsClientAsync();
        public abstract void AuthenticateAsServer();
        public abstract Task AuthenticateAsServerAsync();
    }
}