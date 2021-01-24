using BerkeleyDB;
using System;
using System.Collections.Generic;
using System.Text;
using Zergatul.Security;

namespace Zergatul.Cryptocurrency.Wallets
{
    public class BitcoinCoreWallet
    {
        private List<Key> _keys;

        public BitcoinCoreWallet(string filename, string passphase = null)
        {
            _keys = new List<Key>();

            byte[] masterKey = null;
            using (var db = Database.Open(filename, "main", new DatabaseConfig
            {
                ReadOnly = true
            }))
            {
                using (var cursor = db.Cursor())
                {
                    var keyReader = new DatabaseEntryReader();
                    var valueReader = new DatabaseEntryReader();
                    while (cursor.MoveNext())
                    {
                        keyReader.Init(cursor.Current.Key.Data);
                        valueReader.Init(cursor.Current.Value.Data);

                        string type = Encoding.ASCII.GetString(keyReader.ReadBytes());
                        
                        switch (type)
                        {
                            case "tx":
                                break;

                            case "ckey":
                                _keys.Add(new Key
                                {
                                    PublicKey = keyReader.ReadBytes(),
                                    EncryptedPrivateKey = valueReader.ReadBytes()
                                });
                                break;

                            case "mkey":
                                byte[] encKey = valueReader.ReadBytes();
                                byte[] salt = valueReader.ReadBytes();
                                int derivationMethod = valueReader.ReadInt32();
                                int iterations = valueReader.ReadInt32();
                                byte[] otherParams = valueReader.ReadBytes();
                                masterKey = DecryptMasterKey(passphase, salt, iterations, derivationMethod, encKey);
                                break;

                            case "name":
                                byte[] hash = keyReader.ReadBytes();
                                byte[] name = valueReader.ReadBytes();
                                break;

                            case "version":
                                int version = valueReader.ReadInt32();
                                break;

                            case "minversion":
                                int minversion = valueReader.ReadInt32();
                                break;

                            case "acc":
                                string account = Encoding.ASCII.GetString(keyReader.ReadBytes());
                                int nVersion = valueReader.ReadInt32();
                                byte[] pubKey = valueReader.ReadBytes();
                                Console.WriteLine(account);
                                PrintAddresses(pubKey);
                                Console.WriteLine();
                                break;

                            case "flags":
                            case "keymeta":
                            case "purpose":
                            case "bestblock":
                            case "orderposnext":
                            case "bestblock_nomerkle":
                            case "pool":
                            case "defaultkey":
                            case "cscript":
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            if (masterKey != null)
            {
                foreach (var k in _keys)
                {
                    var p2pkh = new Bitcoin.P2PKHAddress();
                    p2pkh.FromPublicKey(k.PublicKey);
                    Console.Write($"{p2pkh.Value} = ");

                    try
                    {
                        if (k.EncryptedPrivateKey.Length % 16 != 0)
                            throw new InvalidOperationException();

                        byte[] iv = ByteArray.SubArray(DoubleSHA256.Hash(k.PublicKey), 0, 16);
                        k.PrivateKey = AesDecrypt(iv, masterKey, k.EncryptedPrivateKey);

                        // check if key is valid
                        var generator = new Secp256k1PrivateKey(k.PrivateKey);
                        byte[] genPubKey = k.PublicKey.Length == 33 ? generator.ToCompressedPublicKey() : generator.ToUncompressedPublicKey();
                        if (!ByteArray.Equals(k.PublicKey, genPubKey))
                            throw new InvalidOperationException();

                        //Console.WriteLine(BitHelper.BytesToHex(k.PrivateKey));

                        p2pkh = new Bitcoin.P2PKHAddress();
                        p2pkh.FromPrivateKey(new Secp256k1PrivateKey(k.PrivateKey, k.PublicKey.Length == 33));
                        Console.WriteLine(p2pkh.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                Array.Clear(masterKey, 0, masterKey.Length);
            }
        }

        private void PrintAddresses(byte[] pubKey)
        {
            var p2pk = new Bitcoin.P2PKAddress();
            p2pk.FromPublicKey(pubKey);
            Console.WriteLine(p2pk.Value);

            var p2pkh = new Bitcoin.P2PKHAddress();
            p2pkh.FromPublicKey(pubKey);
            Console.WriteLine(p2pkh.Value);

            var p2wpkh = new Bitcoin.P2WPKHAddress();
            p2wpkh.FromPublicKey(pubKey);
            Console.WriteLine(p2wpkh.Value);

            if (pubKey.Length == 33)
            {
                var p2shp2wpkh = new Bitcoin.P2SHP2WPKHAddress();
                p2shp2wpkh.FromPublicKey(pubKey);
                Console.WriteLine(p2shp2wpkh.Value);
            }
        }

        private static byte[] DecryptMasterKey(string passphase, byte[] salt, int iterations, int derivationMethod, byte[] encryptedKey)
        {
            if (derivationMethod != 0)
                throw new NotImplementedException();

            byte[] data = ByteArray.Concat(Encoding.ASCII.GetBytes(passphase), salt);
            for (int i = 0; i < iterations; i++)
            {
                using (var sha512 = MessageDigest.GetInstance(MessageDigests.SHA512))
                {
                    data = sha512.Digest(data);
                }
            }

            byte[] key = ByteArray.SubArray(data, 0, 32);
            byte[] iv = ByteArray.SubArray(data, 32, 16);

            return AesDecrypt(iv, key, encryptedKey);
        }

        private static byte[] AesDecrypt(byte[] iv, byte[] key, byte[] input)
        {
            using (var aes = SymmetricCipher.GetInstance(SymmetricCiphers.AES))
            {
                aes.InitForDecryption(key, new SymmetricCipherParameters
                {
                    Mode = BlockCipherMode.CBC,
                    Padding = Padding.PKCS7,
                    IV = iv
                });
                byte[] result = new byte[input.Length];
                int length = aes.Update(input, input.Length, result);
                if (aes.DoFinal(new byte[16]) != 0)
                    throw new InvalidOperationException();
                return ByteArray.SubArray(result, 0, length);
            }
        }

        private class DatabaseEntryReader
        {
            private byte[] _data;
            private int _position;

            public DatabaseEntryReader()
            {

            }

            public void Init(byte[] data)
            {
                _data = data;
                _position = 0;
            }

            public byte[] ReadBytes()
            {
                int length = _data[_position++];
                byte[] entry = ByteArray.SubArray(_data, _position, length);
                _position += length;
                return entry;
            }

            public int ReadInt32()
            {
                int entry = BitHelper.ToInt32(_data, _position, ByteOrder.LittleEndian);
                _position += 4;
                return entry;
            }
        }

        private class Key
        {
            public byte[] PublicKey;
            public byte[] EncryptedPrivateKey;
            public byte[] PrivateKey;
        }
    }
}