using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate;
using Zergatul.Network.ASN1;
using Zergatul.Network.ASN1.Structures;

namespace Zergatul.Cryptography.Encoding
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc8017#section-9.2
    /// </summary>
    public class EMSA_PKCS1_v1_5
    {
        public AlgorithmIdentifier DigestAlgorithm { get; private set; }
        public byte[] Digest { get; private set; }

        private int _length;

        private EMSA_PKCS1_v1_5()
        {
        }

        public EMSA_PKCS1_v1_5(AlgorithmIdentifier ai, byte[] digest, int length)
        {
            this.DigestAlgorithm = ai;
            this.Digest = digest;
            this._length = length;
        }

        public byte[] ToBytes()
        {
            var di = new DigestInfo(DigestAlgorithm, Digest);
            var T = di.ToBytes();
            int psLength = _length - T.Length - 3;
            if (psLength < 8)
                throw new InvalidOperationException("Intended encoded message length too short");

            byte[] result = new byte[_length];
            result[1] = 1;
            for (int i = 0; i < psLength; i++)
                result[2 + i] = 0xFF;
            Array.Copy(T, 0, result, _length - T.Length, T.Length);

            return result;
        }

        public static EMSA_PKCS1_v1_5 TryParse(byte[] data)
        {
            if (data.Length < 11)
                return null;

            if (data[0] != 0 || data[1] != 1)
                return null;

            int index = 2;
            int psLength = 0;
            while (index < data.Length && data[index] == 0xFF)
            {
                index++;
                psLength++;
            }
            if (psLength < 8)
                return null;

            if (index >= data.Length)
                return null;

            if (data[index] != 0)
                return null;

            index++;

            if (index >= data.Length)
                return null;

            ASN1Element element;

            using (var ms = new MemoryStream(data, index, data.Length - index))
                try
                {
                    element = ASN1Element.ReadFrom(ms);
                    if (ms.Position != ms.Length)
                        return null;
                }
                catch (EndOfStreamException)
                {
                    return null;
                }

            var di = DigestInfo.Parse(element);

            return new EMSA_PKCS1_v1_5
            {
                DigestAlgorithm = di.Algorithm,
                Digest = di.Digest
            };
        }
    }
}