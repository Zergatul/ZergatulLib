using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate;

namespace Zergatul.Network.Tls
{
    internal class SecurityParameters
    {
        ProtocolVersion Version;
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
        public byte[] MasterSecret;
        public byte[] ClientRandom;
        public byte[] ServerRandom;

        public X509Certificate ServerCertificate;

        public List<byte> HandshakeData;
        public byte[] ClientFinishedHandshakeData;
        public byte[] ServerFinishedHandshakeData;

        public bool ExtendedMasterSecret;
    }
}