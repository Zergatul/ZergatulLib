﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric.CipherMode
{
    public abstract class AEADEncryptor
    {
        public abstract AEADCipherData Encrypt(byte[] IV, byte[] data, byte[] aad);
    }
}