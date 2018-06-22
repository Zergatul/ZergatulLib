using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public ECPoint PublicKey { get; private set; }

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

            ECPDSA ecdsa = new ECPDSA();
            ecdsa.Parameters = new ECPDSAParameters(EllipticCurve.secp256k1);
            PublicKey = ecdsa.RecoverPublicKey(GetSignHash(), v, r, s);
            From = new Address();
            From.FromPublicKey(PublicKey);
        }

        public byte[] GetSignHash()
        {
            byte[] unsignedTx = Rlp.Encode(new RlpItem
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
            var keccak = new Keccak256();
            keccak.Update(unsignedTx);
            return keccak.ComputeHash();
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
            return ecdsa.VerifyHash(GetSignHash(), r, s);
        }

        public bool IsSigned => v != 0 && r != null && s != null;
    }

    public class TransactionParseException : Exception
    {

    }
}