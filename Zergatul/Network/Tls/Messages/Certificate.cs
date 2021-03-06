﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate;

namespace Zergatul.Network.Tls.Messages
{
    internal class Certificate : HandshakeBody
    {
        public List<X509Certificate> Certificates = new List<X509Certificate>();

        private int CertificatesLength => Certificates.Sum(c => c.RawData.Length + 3);

        public Certificate() : base(HandshakeType.Certificate) { }

        public override void Read(BinaryReader reader)
        {
            var counter = reader.StartCounter(reader.ReadUInt24());
            while (counter.CanRead)
            {
                Certificates.Add(new X509Certificate(reader.ReadBytes(reader.ReadUInt24())));
            }
        }

        public override void WriteTo(BinaryWriter writer)
        {
            writer.WriteUInt24(CertificatesLength);
            foreach (var cert in Certificates)
            {
                writer.WriteUInt24(cert.RawData.Length);
                writer.WriteBytes(cert.RawData);
            }
        }
    }
}
