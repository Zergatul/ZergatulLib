namespace Zergatul.Security.Zergatul.Tls
{
    public class TlsStreamRecordLayerException : TlsStreamException
    {
        internal TlsStreamRecordLayerException(ErrorCode error)
            : base(error)
        {

        }
    }
}