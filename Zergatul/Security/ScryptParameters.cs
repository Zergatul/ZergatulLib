namespace Zergatul.Security
{
    public class ScryptParameters : KDFParameters
    {
        /// <summary>
        /// Cost parameter
        /// </summary>
        public ulong N;

        /// <summary>
        /// Block size
        /// </summary>
        public int r;

        /// <summary>
        /// Parallelization parameter
        /// </summary>
        public int p;
    }
}