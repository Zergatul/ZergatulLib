using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Zergatul.IO;
using static Zergatul.Security.Zergatul.Tls.TlsConstants;

namespace Zergatul.Security.Zergatul.Tls
{
    class TlsStream : Security.TlsStream
    {
        internal Stream InnerStream;
        internal StateMachine StateMachine;

        private RecordLayer _recordLayer;
        private bool _isServer;
        private byte[] _readBuffer;

        public TlsStream(Stream innerStream)
        {
            if (innerStream == null)
                throw new ArgumentNullException(nameof(innerStream));
            if (!innerStream.CanRead)
                throw new ArgumentException("innerStream.CanRead should be true", nameof(innerStream));
            if (!innerStream.CanWrite)
                throw new ArgumentException("innerStream.CanWrite should be true", nameof(innerStream));

            _recordLayer.Init(this);
            _readBuffer = new byte[TlsCiphertextMaxLength];
        }

        #region TlsStream overrides

        public override void AuthenticateAsClient()
        {
            throw new NotImplementedException();
        }

        public override Task AuthenticateAsClientAsync()
        {
            throw new NotImplementedException();
        }

        public override void AuthenticateAsServer()
        {
            _isServer = true;
            StateMachine.ResetServer();
            ProcessHandshake();
            throw new NotImplementedException();
        }

        public override Task AuthenticateAsServerAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private methods

        private void ProcessHandshake()
        {
            while (StateMachine.State != MessageFlowState.Finished)
            {
                if (StateMachine.State == MessageFlowState.Reading)
                {
                    switch (StateMachine.RState)
                    {
                        case ReadState.ReadHeader:
                            _recordLayer.ReadNext(_readBuffer, 4);
                            break;
                        case ReadState.ReadBody:
                            break;
                        case ReadState.PostProcess:
                            break;
                    }
                }
            }
        }

        #endregion

        #region Stream overrides

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        #region Not supported

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        #endregion

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
            StreamHelper.BeginReadFromTapToApm(this, buffer, offset, count, callback, state);

        public override int EndRead(IAsyncResult asyncResult) => StreamHelper.EndReadFromTapToApm(asyncResult);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
            StreamHelper.BeginWriteFromTapToApm(this, buffer, offset, count, callback, state);

        public override void EndWrite(IAsyncResult asyncResult) => StreamHelper.EndWriteFromTapToApm(asyncResult);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return base.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        #endregion
    }
}