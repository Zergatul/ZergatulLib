using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Asymmetric
{
    public class RSAEncryptionParameters : AbstractParameters
    {
        public RSAEncryptionScheme Scheme = RSAEncryptionScheme.RSAES_PKCS1_v1_5;

        /// <summary>
        /// Used by OAEP
        /// </summary>
        public AbstractHash Hash;
    }

    public enum RSAEncryptionScheme
    {
        Raw,

        /// <summary>
        /// https://tools.ietf.org/html/rfc8017#section-7.2
        /// </summary>
        RSAES_PKCS1_v1_5,

        /// <summary>
        /// Optimal Asymmetric Encryption Padding
        /// <para>https://tools.ietf.org/html/rfc8017#section-7.1</para>
        /// </summary>
        RSAES_OAEP
    }
}