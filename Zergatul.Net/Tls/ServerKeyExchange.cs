using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Net.Tls.Extensions;

namespace Zergatul.Net.Tls
{
    internal class ServerKeyExchange : HandshakeBody
    {
        private TlsStream _stream;

        public ServerDHParams Params;
        public ServerECDHParams ECParams;

        public SignatureAndHashAlgorithm SignAndHashAlgo;
        public byte[] Signature;

        public override ushort Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ServerKeyExchange(TlsStream stream)
        {
            this._stream = stream;
        }

        public override void Read(BinaryReader reader)
        {
            switch (CipherSuiteDetail.Ciphers[_stream.SelectedCipher].KeyExchange)
            {
                case KeyExchangeAlgorithm.DH_anon:
                    throw new NotImplementedException();
                case KeyExchangeAlgorithm.DHE_DSS:
                case KeyExchangeAlgorithm.DHE_RSA:
                    Params = new ServerDHParams
                    {
                        DH_p = reader.ReadBytes(reader.ReadShort()),
                        DH_g = reader.ReadBytes(reader.ReadShort()),
                        DH_Ys = reader.ReadBytes(reader.ReadShort())
                    };
                    SignAndHashAlgo = new SignatureAndHashAlgorithm
                    {
                        Hash = (HashAlgorithm)reader.ReadByte(),
                        Signature = (SignatureAlgorithm)reader.ReadByte()
                    };
                    Signature = reader.ReadBytes(reader.ReadShort());
                    break;
                case KeyExchangeAlgorithm.RSA:
                case KeyExchangeAlgorithm.DH_DSS:
                case KeyExchangeAlgorithm.DH_RSA:
                    // should not have server key exchange
                    throw new TlsStreamException("");
                case KeyExchangeAlgorithm.ECDHE_RSA:
                case KeyExchangeAlgorithm.ECDHE_ECDSA:
                    ECParams = new ServerECDHParams();
                    ECParams.Read(reader);
                    break;
            }
        }

        public override void WriteTo(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
