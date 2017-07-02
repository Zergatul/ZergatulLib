using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Net.Tls.CipherSuites;
using Zergatul.Net.Tls.Extensions;

namespace Zergatul.Net.Tls
{
    internal class ServerHello : HandshakeBody
    {
        public ProtocolVersion ServerVersion;
        public Random Random;
        public byte[] SessionID = new byte[0];
        public CipherSuiteType CipherSuite;
        public List<TlsExtension> Extensions = new List<TlsExtension>();
        public override ushort Length => (ushort)
            (2 +  // ServerVersion
            32 + // Random
            1 + SessionID.Length + // Session
            2 + // CipherSuite
            1 + // Compression Method
            2 + // Extension Length
            ExtensionsLength);
        public override bool Encrypted => false;

        private ushort ExtensionsLength => (ushort)Extensions.Sum(e => 4 + e.Length);

        public override void Read(BinaryReader reader)
        {
            ServerVersion = (ProtocolVersion)reader.ReadShort();
            Random = new Random
            {
                GMTUnixTime = reader.ReadUInt32(),
                RandomBytes = reader.ReadBytes(28)
            };
            SessionID = reader.ReadBytes(reader.ReadByte());
            CipherSuite = (CipherSuiteType)reader.ReadShort();
            var compressionMethod = reader.ReadByte();

            var counter = reader.StartCounter(reader.ReadShort());
            while (counter.CanRead)
            {
                Extensions.Add(new TlsExtension
                {
                    Type = (ExtensionType)reader.ReadShort(),
                    Data = reader.ReadBytes(reader.ReadShort())
                });
            }
        }

        public override void WriteTo(BinaryWriter writer)
        {
            writer.WriteShort((ushort)ServerVersion);
            Random.WriteTo(writer);
            writer.WriteByte((byte)SessionID.Length);
            // TODO: assume no session
            writer.WriteShort((ushort)CipherSuite);
            writer.WriteByte(0); // compression method

            writer.WriteShort(ExtensionsLength);
            foreach (var ext in Extensions)
            {
                writer.WriteShort((ushort)ext.Type);
                writer.WriteShort(ext.Length);
                writer.WriteBytes(ext.Data);
            }
        }
    }
}
