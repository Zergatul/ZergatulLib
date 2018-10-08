using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Http.Frames;

namespace Zergatul.Network.Http
{
    public class Http2Response
    {
        private readonly Http2Client _client;
        private readonly uint _streamId;

        private List<Header> _headers;

        internal Http2Response(Http2Client client, uint streamId)
        {
            this._client = client;
            this._streamId = streamId;

            var frame = client.GetNextFrameForStream(streamId);
            if (frame.Type != FrameType.HEADERS)
                throw new InvalidOperationException();

            var headers = frame as Headers;
            if (!headers.END_HEADERS)
                throw new NotImplementedException();

            _headers = client.DecodeHeaders(headers.Data);
        }
    }
}