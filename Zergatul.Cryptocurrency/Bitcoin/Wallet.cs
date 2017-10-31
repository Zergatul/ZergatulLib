using BerkeleyDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class Wallet
    {
        public IReadOnlyCollection<Key> Keys { get; private set; }
        public bool Error { get; private set; }

        public static Wallet Open(string filename)
        {
            var db = BTreeDatabase.Open(filename, "main", new BTreeDatabaseConfig
            {
                ReadOnly = true,
            });

            var wallet = new Wallet();
            var keys = new List<Key>();
            wallet.Keys = keys.AsReadOnly();

            try
            {
                BTreeCursor cursor = db.Cursor();
                foreach (var entry in cursor)
                {
                    var kreader = new Reader(entry.Key.Data);
                    var vreader = new Reader(entry.Value.Data);
                    string type = kreader.ReadString();
                    switch (type)
                    {
                        case "tx":
                            break;
                        case "key":
                            keys.Add(new Key(kreader.ReadBytes(), vreader.ReadBytes()));
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                wallet.Error = true;
            }

            return wallet;
        }

        public class Key
        {
            public byte[] PublicKey { get; private set; }
            public byte[] PrivateKey { get; private set; }
            public bool IsValid { get; private set; }

            internal Key(byte[] pubkey, byte[] privkey)
            {
                this.PublicKey = pubkey;
                this.PrivateKey = privkey;

                this.IsValid = PublicKey.Length == 33 || PublicKey.Length == 65;
                if (this.IsValid)
                    this.IsValid = Validate();
            }

            private bool Validate()
            {
                var curve = Zergatul.Math.EllipticCurves.PrimeField.EllipticCurve.secp256k1;
                var privkey = new BigInteger(PrivateKey, 0, 32, ByteOrder.BigEndian);
                var pubkey = privkey * curve.g;

                byte[] x = pubkey.x.ToBytes(ByteOrder.BigEndian, 32);
                byte[] y = pubkey.y.ToBytes(ByteOrder.BigEndian, 32);

                if (PublicKey.Length == 33) // compressed
                {
                    for (int i = 0; i < 32; i++)
                        if (PublicKey[i + 1] != x[i])
                            return false;
                }

                if (PublicKey.Length == 65)
                {
                    for (int i = 0; i < 32; i++)
                        if (PublicKey[i + 1] != x[i] || PublicKey[i + 33] != y[i])
                            return false;
                }

                throw new InvalidOperationException();
            }
        }

        private class Reader
        {
            private byte[] _data;
            private int _index;

            public Reader(byte[] data)
            {
                this._data = data;
            }

            public int ReadCompactSize()
            {
                byte size = _data[_index++];
                if (size > 252)
                    throw new NotImplementedException();
                return size;
            }

            public string ReadString()
            {
                int size = ReadCompactSize();
                _index += size;
                return Encoding.ASCII.GetString(_data, _index - size, size);
            }

            public byte[] ReadBytes()
            {
                int size = ReadCompactSize();
                _index += size;
                return ByteArray.SubArray(_data, _index - size, size);
            }
        }
    }
}