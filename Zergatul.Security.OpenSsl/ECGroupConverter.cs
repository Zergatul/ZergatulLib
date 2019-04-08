using System;

namespace Zergatul.Security.OpenSsl
{
    static class ECGroupConverter
    {
        public static int GroupNameToNID(string curve)
        {
            switch (curve)
            {
                case Curves.secp256k1: return Native.NID_secp256k1;
                case Curves.secp256r1: return Native.NID_X9_62_prime256v1;
                case Curves.secp384r1: return Native.NID_secp384r1;
                case Curves.secp521r1: return Native.NID_secp521r1;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}