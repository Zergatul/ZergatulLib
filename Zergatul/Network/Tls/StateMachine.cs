using System;

namespace Zergatul.Network.Tls
{
    internal class StateMachine
    {
        #region Public properties

        public bool IsReading
        {
            get => _isReading;
            private set => _isReading = value;
        }

        public bool IsWriting
        {
            get => !_isReading;
            private set => _isReading = !value;
        }

        public ConnectionState State { get; private set; }
        public ReadState RState { get; private set; }
        public WriteState WState { get; private set; }

        #endregion

        private TlsStreamNew _stream;
        private bool _isReading;

        public StateMachine(TlsStreamNew stream)
        {
            State = ConnectionState.Init;
            _stream = stream;
        }

        public void InitClient()
        {
            if (State != ConnectionState.Init)
                throw new InvalidOperationException();

            State = ConnectionState.ClientHello;
            IsWriting = true;
            WState = WriteState.PreProcess;
        }

        public void InitServer()
        {
            if (State != ConnectionState.Init)
                throw new InvalidOperationException();

            State = ConnectionState.ClientHello;
            IsReading = true;
            RState = ReadState.ReadHeader;
        }

        #region Nested classes

        public enum ConnectionState
        {
            Init,
            ClientHello
        }

        public enum ReadState
        {
            ReadHeader,
            ReadBody,
            Process
        }

        public enum WriteState
        {
            PreProcess,
            Write,
            Flush,
            PostProcess
        }

        #endregion
    }
}