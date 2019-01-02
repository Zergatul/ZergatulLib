using System;
using System.Collections.Generic;
using System.IO;
using Zergatul.Network.Http.Frames;

namespace Zergatul.Network.Http
{
    public class Http2Response
    {
        #region Header properties

        public string ContentEncoding { get; private set; }

        #endregion

        public HttpStatusCode Status { get; private set; }
        public IEnumerable<Header> Headers => _headers;
        public Stream Stream { get; private set; }
        internal uint StreamId { get; private set; }

        private readonly Http2Client _client;

        private List<Header> _headers;

        internal Http2Response(Http2Client client, uint streamId)
        {
            this._client = client;
            this.StreamId = streamId;

            var frame = client.GetNextFrameForStream(streamId);
            if (frame.Type != FrameType.HEADERS)
                throw new InvalidOperationException();

            var headers = frame as HeadersFrame;
            if (!headers.END_HEADERS)
                throw new NotImplementedException();

            _headers = client.DecodeHeaders(headers.Data);
            ProcessHeaders();

            if (headers.END_STREAM)
                Stream = new EmptyReponseStream();
            else
                Stream = new Http2ResponseStream(client, this);
        }

        private void ProcessHeaders()
        {
            bool pseudo = true;
            bool status = false;
            for (int i = 0; i < _headers.Count; i++)
            {
                bool currPseudo = _headers[i].Name[0] == ':';
                if (_headers[i].Name == ":status")
                {
                    if (status)
                        throw new InvalidOperationException();
                    else
                    {
                        status = true;
                        if (int.TryParse(_headers[i].Value, out int result))
                            Status = (HttpStatusCode)result;
                        else
                            throw new InvalidOperationException();
                    }
                }
                if (!pseudo && currPseudo)
                    throw new InvalidOperationException();
                if (pseudo && !currPseudo)
                    pseudo = false;

                if (!pseudo)
                {
                    switch (_headers[i].Name)
                    {
                        case Http2Headers.ContentEncoding:
                            ContentEncoding = _headers[i].Value;
                            break;
                    }
                }
            }
        }
    }
}