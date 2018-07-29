using System;
using Zergatul.Cryptography.Hash.Base;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
    public class Skein512 : AbstractHash
    {
        public override int HashSize => _hashSizeBytes;
        public override int BlockSize => 64;
        public override OID OID => null;

        private Skein_512BitState skein;
        private int _hashSizeBytes;

        public Skein512(int hashSizeBytes = 64)
        {
            ulong[] IV;
            switch (hashSizeBytes)
            {
                case 28: IV = SkeinParameters.IV224; break;
                case 32: IV = SkeinParameters.IV256; break;
                case 48: IV = SkeinParameters.IV384; break;
                case 64: IV = SkeinParameters.IV512; break;
                default:
                    throw new ArgumentException("Invalid hash size");
            }

            skein = new Skein_512BitState(new SkeinParameters
            {
                IV = IV
            });
            _hashSizeBytes = hashSizeBytes;
        }

        public override void Update(byte[] data, int index, int length) => skein.Update(data, index, length);
        public override void Update(byte[] data) => Update(data, 0, data.Length);

        protected override void Init() => skein?.Reset();

        protected override void ProcessBuffer()
        {

        }

        protected override void ProcessBlock()
        {

        }

        protected override void AddPadding()
        {

        }

        protected override byte[] InternalStateToBytes()
        {
            byte[] result = skein.GetResult();
            if (result.Length != _hashSizeBytes)
                result = ByteArray.SubArray(result, 0, HashSize);
            return result;
        }
    }
}