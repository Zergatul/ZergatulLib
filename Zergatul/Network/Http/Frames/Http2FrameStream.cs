using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Http.Frames
{
    class Http2FrameStream
    {
        public StreamState State;

        public Http2FrameStream(Http2Connection connection)
        {

        }

        #region Nested Classes

        public enum StreamState
        {
            Idle,
            ReservedLocal,
            ReservedRemote,
            Open,
            HalfClosedLocal,
            HalfClosedRemote,
            Closed
        }

        #endregion
    }
}