namespace Zergatul.Network.Http
{
    public class Http2ConnectionSettings
    {
        public uint HeaderTableSize = 4096;
        public bool EnablePush = true;
        public uint MaxConcurrentStreams = uint.MaxValue;
        public uint InitialWindowSize = 65535;
        public uint MaxFrameSize = 16384;
        public uint MaxHeaderListSize = uint.MaxValue;
    }
}