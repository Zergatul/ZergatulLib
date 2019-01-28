using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Zergatul.Security
{
    public abstract class SecurityProvider
    {
        #region Instance

        public abstract string Name { get; }

        protected delegate KeyDerivationFunction GetKeyDerivationFunctionDelegate();
        private Dictionary<string, GetKeyDerivationFunctionDelegate> _keyDerivationFunctions = new Dictionary<string, GetKeyDerivationFunctionDelegate>();
        protected delegate KeyPairGenerator GetKeyPairGeneratorDelegate();
        private Dictionary<string, GetKeyPairGeneratorDelegate> _keyPairGenerators = new Dictionary<string, GetKeyPairGeneratorDelegate>();
        protected delegate MessageDigest GetMessageDigestDelegate();
        private Dictionary<string, GetMessageDigestDelegate> _messageDigests = new Dictionary<string, GetMessageDigestDelegate>();
        protected delegate SecureRandom GetSecureRandomDelegate();
        private Dictionary<string, GetSecureRandomDelegate> _secureRandoms = new Dictionary<string, GetSecureRandomDelegate>();
        protected delegate Signature GetSignatureDelegate();
        private Dictionary<string, GetSignatureDelegate> _signatures = new Dictionary<string, GetSignatureDelegate>();
        protected delegate SymmetricCipher GetSymmetricCipherDelegate();
        private Dictionary<string, GetSymmetricCipherDelegate> _symmetricCiphers = new Dictionary<string, GetSymmetricCipherDelegate>();

        public virtual BigInteger GetBigInteger(int value) => null;
        public virtual BigInteger GetBigInteger(long value) => null;
        public virtual BigInteger GetBigInteger(byte[] data, ByteOrder order = ByteOrder.BigEndian) => null;
        public virtual BigInteger GetBigInteger(byte[] data, int offset, int length, ByteOrder order = ByteOrder.BigEndian) => null;
        public virtual BigInteger GetBigInteger(string value, int radix = 10) => null;
        public virtual BigInteger GetBigInteger(string value, int radix, char[] symbols) => null;
        public virtual BigInteger GetBigIntegerRandom(BigInteger value) => null;
        public virtual BigInteger GetBigIntegerRandom(BigInteger value, SecureRandom random) => null;

        public KeyDerivationFunction GetKeyDerivationFunction(string algorithm)
        {
            if (_keyDerivationFunctions.TryGetValue(algorithm.ToUpperInvariant(), out GetKeyDerivationFunctionDelegate getter))
                return getter();
            return null;
        }

        public KeyPairGenerator GetKeyPairGenerator(string algorithm)
        {
            if (_keyPairGenerators.TryGetValue(algorithm.ToUpperInvariant(), out GetKeyPairGeneratorDelegate getter))
                return getter();
            return null;
        }

        public MessageDigest GetMessageDigest(string algorithm)
        {
            if (_messageDigests.TryGetValue(algorithm.ToUpperInvariant(), out GetMessageDigestDelegate getter))
                return getter();
            return null;
        }

        public SecureRandom GetSecureRandom(string algorithm)
        {
            if (_secureRandoms.TryGetValue(algorithm.ToUpperInvariant(), out GetSecureRandomDelegate getter))
                return getter();
            return null;
        }

        public Signature GetSignature(string algorithm)
        {
            if (_signatures.TryGetValue(algorithm.ToUpperInvariant(), out GetSignatureDelegate getter))
                return getter();
            return null;
        }

        public SymmetricCipher GetSymmetricCipher(string algorithm)
        {
            if (_symmetricCiphers.TryGetValue(algorithm.ToUpperInvariant(), out GetSymmetricCipherDelegate getter))
                return getter();
            return null;
        }

        public virtual TlsStream GetTlsStream(Stream innerStream) => null;

        protected void RegisterKeyDerivationFunction(string algorithm, GetKeyDerivationFunctionDelegate getter)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            _keyDerivationFunctions.Add(algorithm.ToUpperInvariant(), getter);
        }

        protected void RegisterKeyPairGenerator(string algorithm, GetKeyPairGeneratorDelegate getter)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            _keyPairGenerators.Add(algorithm.ToUpperInvariant(), getter);
        }

        protected void RegisterMessageDigest(string algorithm, GetMessageDigestDelegate getter)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            _messageDigests.Add(algorithm.ToUpperInvariant(), getter);
        }

        protected void RegisterSecureRandom(string algorithm, GetSecureRandomDelegate getter)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            _secureRandoms.Add(algorithm.ToUpperInvariant(), getter);
        }

        protected void RegisterSignature(string algorithm, GetSignatureDelegate getter)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            _signatures.Add(algorithm.ToUpperInvariant(), getter);
        }

        protected void RegisterSymmetricCipher(string algorithm, GetSymmetricCipherDelegate getter)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            _symmetricCiphers.Add(algorithm.ToUpperInvariant(), getter);
        }

        public void UseAsDefault()
        {
            int index = _providers.IndexOf(this);
            if (index == -1)
            {
                _providers.Insert(0, this);
            }
            if (index > 0)
            {
                var old = _providers[0];
                _providers[0] = _providers[index];
                _providers[index] = old;
            }
        }

        #endregion

        #region Static

        private static List<SecurityProvider> _providers;
        public static IReadOnlyList<SecurityProvider> Providers { get; private set; }
        public static SecurityProvider Default => _providers.Count == 0 ? null : _providers[0];

        static SecurityProvider()
        {
            _providers = new List<SecurityProvider>();
            Providers = _providers.AsReadOnly();

            Register(new DefaultSecurityProvider());
            Register(new DotNetProvider());
            Register(new OpenSslProvider());
            Register(new BouncyCastleProvider());
        }

        public static SecurityProvider Get(string name)
        {
            return _providers.SingleOrDefault(p => string.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static KeyDerivationFunction GetKeyDerivationFunctionInstance(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            for (int i = 0; i < _providers.Count; i++)
            {
                var kdf = _providers[i].GetKeyDerivationFunction(algorithm);
                if (kdf != null)
                    return kdf;
            }

            return null;
        }

        public static KeyPairGenerator GetKeyPairGeneratorInstance(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            for (int i = 0; i < _providers.Count; i++)
            {
                var kpg = _providers[i].GetKeyPairGenerator(algorithm);
                if (kpg != null)
                    return kpg;
            }

            return null;
        }

        public static MessageDigest GetMessageDigestInstance(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            for (int i = 0; i < _providers.Count; i++)
            {
                var md = _providers[i].GetMessageDigest(algorithm);
                if (md != null)
                    return md;
            }

            return null;
        }

        public static SecureRandom GetSecureRandomInstance(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            for (int i = 0; i < _providers.Count; i++)
            {
                var sr = _providers[i].GetSecureRandom(algorithm);
                if (sr != null)
                    return sr;
            }

            return null;
        }

        public static Signature GetSignatureInstance(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            for (int i = 0; i < _providers.Count; i++)
            {
                var signature = _providers[i].GetSignature(algorithm);
                if (signature != null)
                    return signature;
            }

            return null;
        }

        public static SymmetricCipher GetSymmetricCipherInstance(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw new ArgumentNullException(nameof(algorithm));

            for (int i = 0; i < _providers.Count; i++)
            {
                var sc = _providers[i].GetSymmetricCipher(algorithm);
                if (sc != null)
                    return sc;
            }

            return null;
        }

        public static TlsStream GetTlsStreamInstance(Stream innerStream)
        {
            for (int i = 0; i < _providers.Count; i++)
            {
                var tls = _providers[i].GetTlsStream(innerStream);
                if (tls != null)
                    return tls;
            }

            return null;
        }

        public static void Register(SecurityProvider provider) => Register(provider, int.MaxValue);

        public static void Register(SecurityProvider provider, int index)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (_providers.Any(p => p == provider))
                throw new ArgumentException("Provider already registered");
            if (_providers.Any(p => string.Equals(p.Name, provider.Name, StringComparison.InvariantCultureIgnoreCase)))
                throw new ArgumentException("Provider with the same name already registered");

            if (index > _providers.Count)
                index = _providers.Count;

            _providers.Insert(index, provider);
        }

        public static void Unregister(int index)
        {
            if (_providers.Count < index)
                throw new ArgumentException();

            _providers.RemoveAt(0);
        }

        public static void UnregisterAll()
        {
            _providers.Clear();
        }

        #endregion
    }
}