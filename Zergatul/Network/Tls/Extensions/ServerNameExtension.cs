using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    internal class ServerNameExtension : TlsExtension
    {
        private ServerName[] _serverNameList;
        public ServerName[] ServerNameList
        {
            get
            {
                return _serverNameList;
            }
            set
            {
                _serverNameList = value;
                FormatData();
            }
        }

        protected override void ParseData()
        {
            var reader = new BinaryReader(_data);
            var counter = reader.StartCounter(reader.ReadShort());
            var list = new List<ServerName>();
            while (counter.CanRead)
            {
                list.Add(new ServerName
                {
                    NameType = (NameType)reader.ReadByte(),
                    HostName = Encoding.ASCII.GetString(reader.ReadBytes(reader.ReadShort()))
                });
            }

            _serverNameList = list.ToArray();
        }

        protected override void FormatData()
        {
            var data = new List<byte>();
            var writer = new BinaryWriter(data);

            using (writer.WriteShortLengthOfBlock())
            {
                for (int i = 0; i < _serverNameList.Length; i++)
                {
                    writer.WriteByte((byte)_serverNameList[i].NameType);
                    byte[] bytes = Encoding.ASCII.GetBytes(_serverNameList[i].HostName);
                    writer.WriteShort((ushort)bytes.Length);
                    writer.WriteBytes(bytes);
                }
            }

            _data = data.ToArray();
        }
    }

    internal class ServerName
    {
        public NameType NameType;
        public string HostName;
    }

    internal enum NameType : byte
    {
        HostName = 0
    }
}