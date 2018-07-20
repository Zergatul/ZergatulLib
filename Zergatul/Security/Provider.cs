using System;
using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Security
{
    public abstract class Provider
    {
        #region Instance

        public abstract string Name { get; }

        protected delegate MessageDigest GetMessageDigestDelegate();
        private Dictionary<string, GetMessageDigestDelegate> _messageDigests = new Dictionary<string, GetMessageDigestDelegate>();
        protected delegate SecureRandom GetSecureRandomDelegate();
        private Dictionary<string, GetSecureRandomDelegate> _secureRandoms = new Dictionary<string, GetSecureRandomDelegate>();

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

        private static List<Provider> _providers;
        public static IReadOnlyList<Provider> Providers { get; private set; }
        public static Provider Default => _providers.Count == 0 ? null : _providers[0];

        static Provider()
        {
            _providers = new List<Provider>();
            Providers = _providers.AsReadOnly();

            Register(new DefaultProvider());
            Register(new DotNetProvider());
            Register(new OpenSslProvider());
            Register(new BouncyCastleProvider());
        }

        public static Provider Get(string name)
        {
            return _providers.SingleOrDefault(p => string.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        public MessageDigest GetMessageDigest(string algorithm)
        {
            GetMessageDigestDelegate getter;
            if (_messageDigests.TryGetValue(algorithm.ToUpperInvariant(), out getter))
                return getter();
            return null;
        }

        public SecureRandom GetSecureRandom(string algorithm)
        {
            GetSecureRandomDelegate getter;
            if (_secureRandoms.TryGetValue(algorithm.ToUpperInvariant(), out getter))
                return getter();
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

        public static void Register(Provider provider) => Register(provider, int.MaxValue);

        public static void Register(Provider provider, int index)
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

        public static void UnregisterAll()
        {
            _providers.Clear();
        }

        #endregion
    }
}