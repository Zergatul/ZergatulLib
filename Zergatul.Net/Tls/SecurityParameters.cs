using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class SecurityParameters
    {
        //ConnectionEnd entity;
        //PRFAlgorithm prf_algorithm;
        //BulkCipherAlgorithm bulk_cipher_algorithm;
        public CipherType CipherType;
        public byte EncKeyLength;
        public byte BlockLength;
        public byte FixedIVLength;
        public byte RecordIVLength;
        /*MACAlgorithm mac_algorithm;*/
        public byte MACLength;
        /*uint8 mac_key_length;
        CompressionMethod compression_algorithm;*/
        public ByteArray MasterSecret;
        public ByteArray ClientRandom;
        public ByteArray ServerRandom;
    }
}
