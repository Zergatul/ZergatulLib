using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class CipherSuiteDetail
    {
        public static readonly ReadOnlyDictionary<CipherSuite, CipherSuiteDetail> Ciphers = new ReadOnlyDictionary<CipherSuite, CipherSuiteDetail>(new Dictionary<CipherSuite, CipherSuiteDetail>
        {
            [CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384] = new CipherSuiteDetail(KeyExchangeAlgorithm.DHE_RSA),
            [CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384] = new CipherSuiteDetail(KeyExchangeAlgorithm.ECDHE_RSA)
        });

        public KeyExchangeAlgorithm KeyExchange { get; private set; }

        private CipherSuiteDetail(KeyExchangeAlgorithm keyExchange)
        {
            this.KeyExchange = keyExchange;
        }
    }
}