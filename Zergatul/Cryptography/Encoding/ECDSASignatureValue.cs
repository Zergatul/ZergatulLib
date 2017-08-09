using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Encoding
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc4492#page-21
    /// </summary>
    public class ECDSASignatureValue
    {
        public BigInteger r { get; private set; }
        public BigInteger s { get; private set; }


    }
}