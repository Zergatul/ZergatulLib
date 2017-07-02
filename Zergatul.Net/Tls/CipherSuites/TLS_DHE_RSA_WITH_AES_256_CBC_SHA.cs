using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal class TLS_DHE_RSA_WITH_AES_256_CBC_SHA : CipherSuite
    {
        public TLS_DHE_RSA_WITH_AES_256_CBC_SHA(SecurityParameters secParams, Role role, ISecureRandom random) :
            base(secParams, role, random)
        {
            this._keyExchange = new DHEKeyExchange(random);
            this._blockCipher = new AES256(BlockCipherMode.CBC);

            secParams.CipherType = CipherType.Block;
            secParams.EncKeyLength = 32;
            secParams.BlockLength = 16;
            secParams.RecordIVLength = 16;
            secParams.FixedIVLength = 0;
            secParams.MACLength = 20;
        }

        protected override void InitHMAC()
        {
            this._hmac = new HMACSHA1(MACEncryptKey);
        }
    }
}
