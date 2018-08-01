using System;
using Zergatul.Cryptography.Hash.Base;

namespace Zergatul.Cryptography.Hash
{
    public class CubeHash224 : CubeHash
    {
        private static readonly uint[] IV = new uint[]
        {
            0x2AEA2A61, 0x50F494D4, 0x2D538B8B, 0x4167D83E,
            0x3FEE2313, 0xC701CF8C, 0xCC39968E, 0x50AC5695,
            0x4D42C787, 0xA647A8B3, 0x97CF0BEF, 0x825B4537,
            0xEEF864D2, 0xF22090C4, 0xD0E5CD33, 0xA23911AE,
            0xFCD398D9, 0x148FE485, 0x1B017BEF, 0xB6444532,
            0x6A536159, 0x2FF5781C, 0x91FA7934, 0x0DBADEA9,
            0xD65C8A2B, 0xA5A70E75, 0xB1C62456, 0xBC796576,
            0x1921C8F7, 0xE7989AF1, 0x7795D246, 0xD43E3B44,
        };


        public CubeHash224()
            : base(160, 16, 32, 160, 224)
        {

        }

        //protected override void Init()
        //{
        //    Array.Copy(IV, 0, state, 0, 32);
        //}
    }
}