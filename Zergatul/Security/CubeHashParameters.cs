namespace Zergatul.Security
{
    public class CubeHashParameters : MDParameters
    {
        public int InitializationRounds;
        public int RoundsPerBlock;
        public int BytesPerBlock;
        public int FinalizationRounds;
        public int HashSizeBits;
    }
}