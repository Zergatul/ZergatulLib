using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptocurrency.Ethereum
{
    public class Transaction
    {
        public Address From { get; set; }
        public Address To { get; set; }

        public int Nonce { get; set; }
        public long GasPrice { get; set; }
        public long GasLimit { get; set; }
        public byte[] Data { get; set; }

        public byte v { get; set; }
        public byte[] r { get; set; }
        public byte[] s { get; set; }

        public long ValueWei { get; set; }
        public decimal ValueEther
        {
            get
            {
                return ValueWei / 1e18m;
            }
            set
            {
                ValueWei = (long)(value * 1e18m);
            }
        }

        public byte[] Id
        {
            get
            {
                if (!IsSigned)
                    return null;

                var keccak = new Keccak256();
                keccak.Update(Raw);
                byte[] hash = keccak.ComputeHash();

                return ByteArray.SubArray(hash, 0, 32);
            }
        }

        public string IdString
        {
            get
            {
                byte[] id = Id;
                if (id != null)
                    return "0x" + BitHelper.BytesToHex(id);
                else
                    return null;
            }
        }

        public bool IsEIP155 { get; set; }
        public Chain? ChainId { get; set; }

        public byte[] Raw
        {
            get
            {
                if (!IsSigned)
                    return null;
                return Rlp.Encode(new RlpItem
                {
                    Items = new[]
                    {
                        new RlpItem(Nonce),
                        new RlpItem(GasPrice),
                        new RlpItem(GasLimit),
                        new RlpItem(To.Hash),
                        new RlpItem(ValueWei),
                        new RlpItem(Data),
                        new RlpItem(new[] { v }),
                        new RlpItem(r),
                        new RlpItem(s)
                    }
                });
            }
        }

        public string RawString => BitHelper.BytesToHex(Raw);

        public ECPoint PublicKey { get; private set; }

        public bool IsSigned => v != 0 && r != null && s != null;

        public Transaction()
        {
            IsEIP155 = true;
            ChainId = Chain.MainNet;
        }

        public void ParseHex(string hex) => Parse(BitHelper.HexToBytes(hex));

        public void Parse(byte[] data)
        {
            var rdl = Rlp.Decode(data);
            if (rdl.Items?.Length != 9)
                throw new TransactionParseException();

            if (rdl.Items.Any(i => i.String == null))
                throw new TransactionParseException();

            Nonce = (int)(new BigInteger(rdl.Items[0].String, ByteOrder.BigEndian));
            GasPrice = (long)(new BigInteger(rdl.Items[1].String, ByteOrder.BigEndian));
            GasLimit = (long)(new BigInteger(rdl.Items[2].String, ByteOrder.BigEndian));
            To = new Address();
            To.FromHash(rdl.Items[3].String);
            ValueWei = (long)(new BigInteger(rdl.Items[4].String, ByteOrder.BigEndian));
            Data = rdl.Items[5].String;

            if (rdl.Items[6].String.Length != 1)
                throw new TransactionParseException();
            if (rdl.Items[7].String.Length > 32 || rdl.Items[8].String.Length > 32)
                throw new TransactionParseException();

            v = rdl.Items[6].String[0];
            r = rdl.Items[7].String;
            s = rdl.Items[8].String;

            if (27 <= v && v <= 28)
            {
                IsEIP155 = false;
                ChainId = null;
            }
            else if (v >= 37)
            {
                IsEIP155 = true;
                if (ChainId == null)
                    throw new InvalidOperationException();
            }

            ECPDSA ecdsa = new ECPDSA();
            ecdsa.Parameters = new ECPDSAParameters(EllipticCurve.secp256k1);
            PublicKey = ecdsa.RecoverPublicKey(GetSignHash(), GetRecId(), r, s);
            From = new Address();
            From.FromPublicKey(PublicKey);
        }

        public byte[] GetSignHash()
        {
            byte[] unsignedTx;
            if (IsEIP155)
            {
                unsignedTx = Rlp.Encode(new RlpItem
                {
                    Items = new[]
                    {
                        new RlpItem(Nonce),
                        new RlpItem(GasPrice),
                        new RlpItem(GasLimit),
                        new RlpItem(To.Hash),
                        new RlpItem(ValueWei),
                        new RlpItem(Data),
                        new RlpItem((int)ChainId.Value),
                        new RlpItem(0),
                        new RlpItem(0)
                    }
                });
            }
            else
            {
                unsignedTx = Rlp.Encode(new RlpItem
                {
                    Items = new[]
                    {
                        new RlpItem(Nonce),
                        new RlpItem(GasPrice),
                        new RlpItem(GasLimit),
                        new RlpItem(To.Hash),
                        new RlpItem(ValueWei),
                        new RlpItem(Data)
                    }
                });
            }

            var keccak = new Keccak256();
            keccak.Update(unsignedTx);
            return keccak.ComputeHash();
        }

        public void Sign(byte[] key)
        {
            if (key.Length != 32)
                throw new ArgumentException();
            if (IsEIP155 && ChainId == null)
                throw new InvalidOperationException();

            ECPDSA ecdsa = new ECPDSA();
            ecdsa.Parameters = new ECPDSAParameters(EllipticCurve.secp256k1);
            ecdsa.Parameters.LowS = true;
            ecdsa.Random = new SecureRandomWrapper();
            ecdsa.PrivateKey = new ECPPrivateKey(key);
            ecdsa.SignHashWithRecovery(GetSignHash(), out byte v, out BigInteger r, out BigInteger s);

            if (v >= 2)
                throw new InvalidOperationException();
            if (IsEIP155)
                this.v = (byte)(35 + (int)ChainId * 2 + v);
            else
                this.v = (byte)(27 + v);
            this.r = r.ToBytes(ByteOrder.BigEndian);
            this.s = s.ToBytes(ByteOrder.BigEndian);
        }

        public bool VerifySignature()
        {
            if (!IsSigned)
                return false;
            if (PublicKey == null)
                return false;

            ECPDSA ecdsa = new ECPDSA();
            ecdsa.Parameters = new ECPDSAParameters(EllipticCurve.secp256k1);
            ecdsa.PublicKey = new ECPPublicKey(PublicKey);
            if ((s[0] & 0x80) != 0) // low S rule
                return false;
            return ecdsa.VerifyHash(GetSignHash(), r, s);
        }

        private byte GetRecId()
        {
            byte recId;
            if (IsEIP155)
                recId = checked((byte)(v - (int)ChainId * 2 - 35));
            else
                recId = checked((byte)(v - 27));

            if (recId >= 2)
                throw new InvalidOperationException();

            return recId;
        }
    }

    public class TransactionParseException : Exception
    {

    }
}