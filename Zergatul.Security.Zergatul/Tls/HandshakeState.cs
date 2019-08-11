namespace Zergatul.Security.Zergatul.Tls
{
    enum HandshakeState
    {
        Init,
        ClientWriteClientHello,
        ServerReadClientHello,

        Finished
    }
}