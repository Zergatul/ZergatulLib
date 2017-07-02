﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal abstract class AbstractKeyExchange
    {
        public ByteArray PreMasterSecret;

        public abstract void GetServerKeyExchange(ServerKeyExchange message);
        public abstract void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader);
        public abstract void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer);

        public abstract void GetClientKeyExchange(ClientKeyExchange message);
        public abstract void ReadClientKeyExchange(ClientKeyExchange message, BinaryReader reader);
        public abstract void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer);
    }
}