using System;

namespace Zergatul.Security.OpenSsl
{
    static class ECGroupConverter
    {
        public static int GroupNameToNID(string curve)
        {
            switch (curve)
            {
                case Curves.secp256k1: return OpenSsl.NID_secp256k1;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}