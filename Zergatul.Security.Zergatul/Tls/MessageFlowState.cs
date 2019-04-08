namespace Zergatul.Security.Zergatul.Tls
{
    enum MessageFlowState
    {
        Init,
        Reading,
        Writing,
        Error,
        Finished
    }
}