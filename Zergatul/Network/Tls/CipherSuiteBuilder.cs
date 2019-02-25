using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Hash;
using Zergatul.Network.Tls.Extensions;
using Zergatul.Network.Tls.Messages;
using Zergatul.Security;
using Zergatul.Security.Tls;

namespace Zergatul.Network.Tls
{
    internal partial class CipherSuiteBuilder
    {
        public SecurityParameters SecurityParameters { get; private set; }
        public Role Role { get; private set; }
        public Role OtherRole => (Role)((int)Role ^ 1);

        public AbstractTlsKeyExchange KeyExchange { get; private set; }
        public AbstractTlsSymmetricCipher SymmetricCipher { get; private set; }
        public AbstractHash PRFHash { get; private set; }
        public TlsConnectionKeys Keys { get; private set; }

        private SecureRandom _random;

        #region Contructors

        public CipherSuiteBuilder(AbstractTlsKeyExchange keyExchange, AbstractTlsSymmetricCipher symmetricCipher, AbstractHash prfHash)
        {
            this.KeyExchange = keyExchange;
            this.SymmetricCipher = symmetricCipher;
            this.PRFHash = prfHash;
        }

        #endregion

        #region Handshake

        public virtual Finished GetFinished()
        {
            return new Finished { Data = GetFinishedVerifyData(Role) };
        }

        public bool VerifyFinished(Finished message)
        {
            byte[] data = GetFinishedVerifyData(OtherRole);
            return data.SequenceEqual(message.Data);
        }

        #endregion

        public void Init(SecurityParameters securityParameters, TlsStreamParameters parameters, Role role, SecureRandom random)
        {
            this.SecurityParameters = securityParameters;
            this.Role = role;
            this._random = random;

            this.KeyExchange.SecurityParameters = securityParameters;
            this.KeyExchange.Parameters = parameters;
            this.KeyExchange.SetRandom(random);

            this.SymmetricCipher.SecurityParameters = securityParameters;
            this.SymmetricCipher.Random = random;
            this.SymmetricCipher.ApplySecurityParameters();
        }

        public virtual void CalculateMasterSecret()
        {
            // RFC 5246
            // https://tools.ietf.org/html/rfc5246#section-8.1
            // The master secret is always exactly 48 bytes in length.
            if (SecurityParameters.ExtendedMasterSecret)
            {
                var sessionHash = Hash(SecurityParameters.HandshakeBuffer.ToArray());
                SecurityParameters.MasterSecret = PseudoRandomFunction(
                    KeyExchange.PreMasterSecret,
                    "extended master secret",
                    sessionHash,
                    48);
            }
            else
            {
                SecurityParameters.MasterSecret = PseudoRandomFunction(
                    KeyExchange.PreMasterSecret,
                    "master secret",
                    ByteArray.Concat(SecurityParameters.ClientRandom, SecurityParameters.ServerRandom),
                    48);
            }

            // RFC 5246
            // https://tools.ietf.org/html/rfc5246#section-8.1
            // The pre_master_secret should be deleted from memory once the master_secret has been computed.
            for (int i = 0; i < KeyExchange.PreMasterSecret.Length; i++)
                KeyExchange.PreMasterSecret[i] = 0;
        }

        public virtual void GenerateKeyMaterial()
        {
            // RFC 5246 // Page 25
            /*
                To generate the key material, compute

                key_block = PRF(SecurityParameters.master_secret,
                      "key expansion",
                      SecurityParameters.server_random +
                      SecurityParameters.client_random);
            */
            var keyBlock = PseudoRandomFunction(
                SecurityParameters.MasterSecret,
                "key expansion",
                ByteArray.Concat(SecurityParameters.ServerRandom, SecurityParameters.ClientRandom),
                2 * (SecurityParameters.MACLength + SecurityParameters.EncKeyLength + SecurityParameters.FixedIVLength));

            // RFC 5246 // Page 25
            /*
                Then, the key_block is partitioned as follows:

                    client_write_MAC_key[SecurityParameters.mac_key_length]
                    server_write_MAC_key[SecurityParameters.mac_key_length]
                    client_write_key[SecurityParameters.enc_key_length]
                    server_write_key[SecurityParameters.enc_key_length]
                    client_write_IV[SecurityParameters.fixed_iv_length]
                    server_write_IV[SecurityParameters.fixed_iv_length]
             */

            int position = 0;
            Keys = new TlsConnectionKeys();

            Keys.ClientMACkey = ByteArray.SubArray(keyBlock, position, SecurityParameters.MACLength);
            position += SecurityParameters.MACLength;
            Keys.ServerMACkey = ByteArray.SubArray(keyBlock, position, SecurityParameters.MACLength);
            position += SecurityParameters.MACLength;
            Keys.ClientEncKey = ByteArray.SubArray(keyBlock, position, SecurityParameters.EncKeyLength);
            position += SecurityParameters.EncKeyLength;
            Keys.ServerEncKey = ByteArray.SubArray(keyBlock, position, SecurityParameters.EncKeyLength);
            position += SecurityParameters.EncKeyLength;
            Keys.ClientIV = ByteArray.SubArray(keyBlock, position, SecurityParameters.FixedIVLength);
            position += SecurityParameters.FixedIVLength;
            Keys.ServerIV = ByteArray.SubArray(keyBlock, position, SecurityParameters.FixedIVLength);
            position += SecurityParameters.FixedIVLength;

            SymmetricCipher.Init(Keys, Role);
        }

        #region Private/Protected methods

        public virtual byte[] Hash(byte[] data)
        {
            PRFHash.Reset();
            PRFHash.Update(data);
            return PRFHash.ComputeHash();
        }

        protected byte[] HMACHash(byte[] secret, byte[] seed)
        {
            var hmac = new HMAC(PRFHash, secret);
            return hmac.ComputeHash(seed);
        }

        protected byte[] PHash(byte[] secret, byte[] seed, int length)
        {
            // RFC 5246 // Page 14
            /*
                P_hash(secret, seed) = HMAC_hash(secret, A(1) + seed) +
                                       HMAC_hash(secret, A(2) + seed) +
                                       HMAC_hash(secret, A(3) + seed) + ...
                A() is defined as:
                     A(0) = seed
                     A(i) = HMAC_hash(secret, A(i - 1))
            */
            byte[] a = seed;
            byte[] result = new byte[length];
            int index = 0;
            while (index < length)
            {
                a = HMACHash(secret, a);
                byte[] hmac = HMACHash(secret, ByteArray.Concat(a, seed));
                int copyLen = System.Math.Min(hmac.Length, result.Length - index);
                Array.Copy(hmac, 0, result, index, copyLen);
                index += copyLen;
            }
            return result;
        }

        protected virtual byte[] PseudoRandomFunction(byte[] secret, string label, byte[] seed, int length)
        {
            // RFC 5246 // Page 14
            // PRF(secret, label, seed) = P_<hash>(secret, label + seed)
            return PHash(secret, ByteArray.Concat(Encoding.ASCII.GetBytes(label), seed), length);
        }

        protected virtual byte[] GetFinishedVerifyData(Role role)
        {
            // RFC 5246
            // https://tools.ietf.org/html/rfc5246#section-7.4.9
            /*
                In previous versions of TLS, the verify_data was always 12 octets
                long.  In the current version of TLS, it depends on the cipher
                suite.  Any cipher suite which does not explicitly specify
                verify_data_length has a verify_data_length equal to 12.  This
                includes all existing cipher suites.  Note that this
                representation has the same encoding as with previous versions.
                Future cipher suites MAY specify other lengths but such length
                MUST be at least 12 bytes.
            */
            string label = role == Role.Client ? "client finished" : "server finished";
            byte[] hash = role == Role.Client ? SecurityParameters.ClientFinishedHash : SecurityParameters.ServerFinishedHash;
            return PseudoRandomFunction(SecurityParameters.MasterSecret, label, hash, 12);
        }

        #endregion
    }
}