using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class Certificate : HandshakeBody
    {
        public List<X509Certificate2> Certificates = new List<X509Certificate2>();
        public override ushort Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override bool Encrypted => false;

        public override void Read(BinaryReader reader)
        {
            var counter = reader.StartCounter(reader.ReadUInt24());
            while (counter.CanRead)
            {
                Certificates.Add(new X509Certificate2(reader.ReadBytes(reader.ReadUInt24())));
            }
        }

        public override void WriteTo(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
