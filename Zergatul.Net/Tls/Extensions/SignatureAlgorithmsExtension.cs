using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.Extensions
{
    internal class SignatureAlgorithmsExtension : TlsExtension
    {
        private SignatureAndHashAlgorithm[] _supportedAlgorithms;
        public SignatureAndHashAlgorithm[] SupportedAlgorithms
        {
            get
            {
                return _supportedAlgorithms;
            }
            set
            {
                _supportedAlgorithms = value;
                FormatData();
            }
        }

        public SignatureAlgorithmsExtension()
            : base(ExtensionType.SignatureAlgorithms)
        {

        }

        public SignatureAlgorithmsExtension(BinaryReader reader)
            : base(ExtensionType.SignatureAlgorithms, reader)
        {
            
        }

        protected override void ParseData()
        {
            var reader = new BinaryReader(_data);
            var counter = reader.StartCounter(reader.ReadShort());
            var list = new List<SignatureAndHashAlgorithm>();
            while (counter.CanRead)
            {
                list.Add(new SignatureAndHashAlgorithm
                {
                    Hash = (HashAlgorithm)reader.ReadByte(),
                    Signature = (SignatureAlgorithm)reader.ReadByte()
                });
            }

            _supportedAlgorithms = list.ToArray();
        }

        protected override void FormatData()
        {
            var data = new List<byte>();
            var writer = new BinaryWriter(data);

            writer.WriteShort((ushort)(_supportedAlgorithms.Length * 2));
            for (int i = 0; i < _supportedAlgorithms.Length; i++)
            {
                writer.WriteByte((byte)_supportedAlgorithms[i].Hash);
                writer.WriteByte((byte)_supportedAlgorithms[i].Signature);
            }

            _data = data.ToArray();
        }
    }
}