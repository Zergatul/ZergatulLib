using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate;

namespace Zergatul.Network.Asn1.Structures
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

        public static EMSA_PKCS1_v1_5 Parse(byte[] data)
        {
            if (data.Length < 11)
                throw new ParseException();

            if (data[0] != 0 || data[1] != 1)
                throw new ParseException();

            int index = 2;
            int psLength = 0;
            while (index < data.Length && data[index] == 0xFF)
            {
                index++;
                psLength++;
            }
            if (psLength < 8)
                throw new ParseException();

            if (index >= data.Length)
                throw new ParseException();

            if (data[index] != 0)
                throw new ParseException();

            index++;

            if (index >= data.Length)
                throw new ParseException();

            Asn1Element element;

            using (var ms = new MemoryStream(data, index, data.Length - index))
                try
                {
                    element = Asn1Element.ReadFrom(ms);
                    if (ms.Position != ms.Length)
                        throw new ParseException();
                }
                catch (EndOfStreamException)
                {
                    throw new ParseException();
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