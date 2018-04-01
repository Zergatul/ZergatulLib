namespace Zergatul.Network.Tls
{
    internal enum ConnectionState
    {
        NoConnection,
        Start,
        ClientHello,
        ServerHello,
        ServerCertificate,
        ServerKeyExchange,
        CertificateRequest,
        ServerHelloDone,
        ClientCertificate,
        ClientKeyExchange,
        CertificateVerify,
        ClientChangeCipherSpec,
        ClientFinished,
        ServerChangeCipherSpec,
        ServerFinished,
        ApplicationData,
        Closed
    }
}