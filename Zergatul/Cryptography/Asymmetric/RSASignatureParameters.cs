using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Asymmetric
{
    public class RSASignatureParameters : AbstractParameters
    {
        public RSASignatureScheme Scheme;
        public AbstractHash Hash;
    }

    public enum RSASignatureScheme
    {
        /// <summary>
        /// https://tools.ietf.org/html/rfc8017#section-8.1
        /// </summary>
        RSASSA_PSS,
        /// <summary>
        /// https://tools.ietf.org/html/rfc8017#section-8.2
        /// </summary>
        RSASSA_PKCS1_v1_5
    }
}