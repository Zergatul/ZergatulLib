using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal static class DHERoutine
    {
        public static DiffieHellman GenerateServerKeyExchange(ServerKeyExchange message, ISecureRandom random, TlsStreamSettings settings)
        {
            var dh = new DiffieHellman();
            dh.Random = random;

            if (settings.DHParameters != null)
                dh.Parameters = settings.DHParameters;
            else
                dh.Parameters = TlsStreamSettings.Default.DHParameters;

            dh.GenerateKeyPair(0);

            message.Params = new ServerDHParams
            {
                DH_p = dh.Parameters.p.ToBytes(ByteOrder.BigEndian),
                DH_g = dh.Parameters.g.ToBytes(ByteOrder.BigEndian),
                DH_Ys = dh.PublicKey.Value_Raw
            };

            return dh;
        }

        public static void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader)
        {
            message.Params = new ServerDHParams();
            message.Params.Read(reader);
        }

        public static byte[] GetSharedSecretAsClient(ServerKeyExchange message, ISecureRandom random, out DiffieHellman dh)
        {
            dh = new DiffieHellman();
            dh.Random = random;
            dh.Parameters = new DiffieHellmanParameters(message.Params.DH_g, message.Params.DH_p);
            dh.GenerateKeyPair(0);
            return dh.CalculateSharedSecret(new DiffieHellmanPublicKey(message.Params.DH_Ys));
        }

        public static void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            writer.WriteBytes(message.Params.ToBytes());
        }

        public static void GenerateClientKeyExchange(ClientKeyExchange message, DiffieHellman dh)
        {
            message.DH_Yc = dh.PublicKey.Value_Raw;
        }

        public static byte[] ReadClientKeyExchange(ClientKeyExchange message, BinaryReader reader, DiffieHellman dh)
        {
            message.DH_Yc = reader.ReadBytes(reader.ReadShort());

            return dh.CalculateSharedSecret(new DiffieHellmanPublicKey(message.DH_Yc));
        }

        public static void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.DH_Yc.Length);
            writer.WriteBytes(message.DH_Yc);
        }
    }
}