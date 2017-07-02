using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Net.Tls.CipherSuites;
using Zergatul.Net.Tls.Extensions;

namespace Zergatul.Net.Tls
{
    internal class ClientHello : HandshakeBody
    {
        public ProtocolVersion ClientVersion;
        public Random Random;
        public byte[] SessionID = new byte[0];
        public CipherSuiteType[] CipherSuites;
        public TlsExtension[] Extensions = new TlsExtension[0];

        public override ushort Length => (ushort)
            (2 + // Version
            32 + // Random
            1 + SessionID.Length + // SessionID
            CipherSuites.Length * 2 + 2 + // CipherSuites
            2 + // Compression Methods
            2 + ExtensionsLength); // Extensions
        public override bool Encrypted => false;

        private ushort ExtensionsLength => (ushort)Extensions.DefaultIfEmpty().Sum(e => 4 + e.Length);

        public override void Read(BinaryReader reader)
        {
            ClientVersion = (ProtocolVersion)reader.ReadShort();

            Random = new Random();
            Random.ReadFrom(reader);

            byte sessionIDLength = reader.ReadByte();
            if (sessionIDLength == 0)
                SessionID = null;
            else
                throw new NotImplementedException();

            ushort cipherSuitesLength = reader.ReadShort();
            if (cipherSuitesLength % 2 == 1 || cipherSuitesLength == 0)
                throw new TlsStreamException("Invalid CipherSuites Length");
            CipherSuites = new CipherSuiteType[cipherSuitesLength / 2];
            for (int i = 0; i < CipherSuites.Length; i++)
                CipherSuites[i] = (CipherSuiteType)reader.ReadShort();

            // Compression methods, skip for now
            byte compressionMethodsLength = reader.ReadByte();
            for (int i = 0; i < compressionMethodsLength; i++)
                reader.ReadByte();

            ushort extensionLength = reader.ReadShort();
            var counter = reader.StartCounter(extensionLength);
            while (counter.CanRead)
            {
                var ext = new TlsExtension();
                ext.Type = (ExtensionType)reader.ReadShort();
                ushort extLength = reader.ReadShort();
                ext.Data = reader.ReadBytes(extLength);
            }
        }

        public override void WriteTo(BinaryWriter writer)
        {
            writer.WriteShort((ushort)ClientVersion);

            Random.WriteTo(writer);

            if (SessionID != null)
            {
                writer.WriteByte((byte)SessionID.Length);
                writer.WriteBytes(SessionID);
            }
            else
                writer.WriteByte(0);

            writer.WriteShort((ushort)(CipherSuites.Length * 2));
            for (int i = 0; i < CipherSuites.Length; i++)
                writer.WriteShort((ushort)CipherSuites[i]);

            // Compression methods, skip for now
            writer.WriteByte(1);
            writer.WriteByte(0);

            writer.WriteShort(ExtensionsLength);
            for (int i = 0; i < Extensions.Length; i++)
            {
                writer.WriteShort((ushort)Extensions[i].Type);
                writer.WriteShort(Extensions[i].Length);
                writer.WriteBytes(Extensions[i].Data);
            }
        }
    }
}
