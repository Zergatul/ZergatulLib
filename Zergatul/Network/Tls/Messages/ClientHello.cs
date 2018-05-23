using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls.Messages
{
    internal class ClientHello : HandshakeBody
    {
        public ProtocolVersion ClientVersion;
        public byte[] Random;
        public byte[] SessionID = new byte[0];
        public CipherSuite[] CipherSuites;
        public TlsExtension[] Extensions = new TlsExtension[0];

        private ushort ExtensionsLength => (ushort)Extensions.DefaultIfEmpty().Sum(e => 4 + e.Length);

        public ClientHello() : base(HandshakeType.ClientHello) { }

        public override void Read(BinaryReader reader)
        {
            ClientVersion = (ProtocolVersion)reader.ReadShort();

            Random = reader.ReadBytes(32);

            byte sessionIDLength = reader.ReadByte();
            if (sessionIDLength == 0)
                SessionID = null;
            else
                throw new NotImplementedException();

            ushort cipherSuitesLength = reader.ReadShort();
            if (cipherSuitesLength % 2 == 1 || cipherSuitesLength == 0)
                throw new TlsStreamException("Invalid CipherSuites Length");
            CipherSuites = new CipherSuite[cipherSuitesLength / 2];
            for (int i = 0; i < CipherSuites.Length; i++)
                CipherSuites[i] = (CipherSuite)reader.ReadShort();

            // Compression methods, skip for now
            byte compressionMethodsLength = reader.ReadByte();
            for (int i = 0; i < compressionMethodsLength; i++)
                reader.ReadByte();

            ushort extensionLength = reader.ReadShort();
            var counter = reader.StartCounter(extensionLength);
            var extensions = new List<TlsExtension>();
            while (counter.CanRead)
            {
                var type = (ExtensionType)reader.ReadShort();
                var ext = TlsExtension.Resolve(type);
                ushort extLength = reader.ReadShort();
                ext.Data = reader.ReadBytes(extLength);
                extensions.Add(ext);
            }
            Extensions = extensions.ToArray();
        }

        public override void WriteTo(BinaryWriter writer)
        {
            writer.WriteShort((ushort)ClientVersion);

            writer.WriteBytes(Random);

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
