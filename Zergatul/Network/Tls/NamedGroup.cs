using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    public enum NamedGroup : ushort
    {
        sect163k1 = 1,
        sect163r1 = 2,
        sect163r2 = 3,
        sect193r1 = 4,
        sect193r2 = 5,
        sect233k1 = 6,
        sect233r1 = 7,
        sect239k1 = 8,
        sect283k1 = 9,
        sect283r1 = 10,
        sect409k1 = 11,
        sect409r1 = 12,
        sect571k1 = 13,
        sect571r1 = 14,
        secp160k1 = 15,
        secp160r1 = 16,
        secp160r2 = 17,
        secp192k1 = 18,
        secp192r1 = 19,
        secp224k1 = 20,
        secp224r1 = 21,
        secp256k1 = 22,
        secp256r1 = 23,
        secp384r1 = 24,
        secp521r1 = 25,

        ffdhe2048 = 256,
        ffdhe3072 = 257,
        ffdhe4096 = 258,
        ffdhe6144 = 259,
        ffdhe8192 = 260,

        ArbitraryExplicitPrimeCurves = 0xFF01,
        ArbitraryExplicitChar2Curves = 0xFF02
    }
}