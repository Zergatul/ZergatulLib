using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc7301
    /// </summary>
    class ApplicationLayerProtocolNegotiationExtension : TlsExtension
    {
        private string[] _prorocolNames;
        public string[] ProtocolNames
        {
            get => (string[])_prorocolNames.Clone();
            set
            {
                _prorocolNames = (string[])value.Clone();
                FormatData();
            }
        }

        public ApplicationLayerProtocolNegotiationExtension()
            : base(ExtensionType.ApplicationLayerProtocolNegotiation)
        {

        }

        protected override void FormatData()
        {
            var data = new List<byte>();
            var writer = new BinaryWriter(data);

            if (_prorocolNames.Length == 0)
                throw new NotImplementedException();

            for (int i = 0; i < _prorocolNames.Length; i++)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(_prorocolNames[i]);
                if (buffer.Length > 0xFF)
                    throw new InvalidOperationException();
                writer.WriteByte((byte)buffer.Length);
                writer.WriteBytes(buffer);
            }

            var data2 = new List<byte>(data.Count + 2);
            writer = new BinaryWriter(data2);
            writer.WriteShort((ushort)data.Count);
            writer.WriteBytes(data.ToArray());

            _data = data2.ToArray();
        }

        protected override void ParseData()
        {
            var reader = new BinaryReader(_data);
            var counter = reader.StartCounter(reader.ReadShort());
            var list = new List<string>();
            while (counter.CanRead)
            {
                int length = reader.ReadByte();
                list.Add(Encoding.UTF8.GetString(reader.ReadBytes(length)));
            }

            _prorocolNames = list.ToArray();
        }
    }
}