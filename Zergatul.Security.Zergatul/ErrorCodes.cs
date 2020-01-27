namespace Zergatul.Security.Zergatul
{
    internal static class ErrorCodes
    {
        private static int _index = 1;

        public static readonly ErrorCode UnexpectedEndOfStream = new ErrorCode(_index++, "Unexpected end of stream.");
        public static readonly ErrorCode InvalidContentType = new ErrorCode(_index++, "Invalid content type.");
        public static readonly ErrorCode RecordLayerOverflow = new ErrorCode(_index++, "Record layer overflow.");
        public static readonly ErrorCode UnexpectedHandshakeMessage = new ErrorCode(_index++, "Received handshake type record, but handshake is finished.");
    }
}