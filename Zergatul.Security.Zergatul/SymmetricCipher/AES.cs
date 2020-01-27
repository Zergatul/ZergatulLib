using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Security.Zergatul.SymmetricCipher
{
    class AES : Security.SymmetricCipher
    {
        #region SymmetricCipher overrides

        public override int BlockSize => 16;

        public override void InitForEncryption(byte[] key, SymmetricCipherParameters parameters)
        {
            throw new NotImplementedException();
        }

        public override void InitForDecryption(byte[] key, SymmetricCipherParameters parameters)
        {
            throw new NotImplementedException();
        }

        public override int Update(byte[] input, int inputLength, byte[] output)
        {
            throw new NotImplementedException();
        }

        public override int DoFinal(byte[] output)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}