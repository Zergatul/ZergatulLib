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

            dh.GenerateKeys();

            message.Params = new ServerDHParams
            {
                DH_p = dh.Parameters.p.ToBytes(ByteOrder.BigEndian),
                DH_g = dh.Parameters.g.ToBytes(ByteOrder.BigEndian),
                DH_Ys = dh.PublicKey.ToBytes(ByteOrder.BigEndian)
            };

            return dh;
        }

        public static void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader)
        {
            message.Params = new ServerDHParams();
            message.Params.Read(reader);
        }

        public static DiffieHellman GetSharedSecretAsClient(ServerKeyExchange message, ISecureRandom random)
        {
            var dh = new DiffieHellman();
            dh.Random = random;
            dh.Parameters = new DiffieHellmanParameters(new BigInteger(message.Params.DH_g, ByteOrder.BigEndian), new BigInteger(message.Params.DH_p, ByteOrder.BigEndian));
            dh.GenerateKeys();
            dh.KeyExchange.CalculateSharedSecret(new BigInteger(message.Params.DH_Ys, ByteOrder.BigEndian));
            return dh;
        }

        public static void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            writer.WriteBytes(message.Params.ToBytes());
        }

        public static byte[] GenerateClientKeyExchange(ClientKeyExchange message, DiffieHellman dh)
        {
            message.DH_Yc = dh.PublicKey.ToBytes(ByteOrder.BigEndian);
            return dh.KeyExchange.SharedSecret.ToBytes(ByteOrder.BigEndian);
        }

        public static byte[] ReadClientKeyExchange(ClientKeyExchange message, BinaryReader reader, DiffieHellman dh)
        {
            message.DH_Yc = reader.ReadBytes(reader.ReadShort());

            dh.KeyExchange.CalculateSharedSecret(new BigInteger(message.DH_Yc, ByteOrder.BigEndian));

            return dh.KeyExchange.SharedSecret.ToBytes(ByteOrder.BigEndian);
        }

        public static void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            writer.WriteShort((ushort)message.DH_Yc.Length);
            writer.WriteBytes(message.DH_Yc);
        }
    }
}