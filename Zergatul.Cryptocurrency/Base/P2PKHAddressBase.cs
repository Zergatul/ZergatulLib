using System;
using Zergatul.Security;

namespace Zergatul.Cryptocurrency.Base
{
    /// <summary>
    /// Pay-to-pubkey-hash
    /// </summary>
    public abstract class P2PKHAddressBase : Base58AddressBase
    {
        public Secp256k1PrivateKey PrivateKey { get; set; }
        public bool? IsCompressed { get; set; }

        protected abstract byte _wifPrefix { get; }

        protected BlockchainCryptoFactory _factory;

        protected P2PKHAddressBase(BlockchainCryptoFactory factory)
        {
            this._factory = factory;
        }

        public void Parse(string address)
        {
            string old = _value;
            _value = address;
            if (!Validate())
            {
                _value = old;
                throw new InvalidOperationException("Invalid address");
            }
        }

        public void FromPublicKey(byte[] pubkeyData)
        {
            FromPublicKeyHash(RIPE160SHA256.Hash(pubkeyData));
        }

        public void FromPublicKeyHash(byte[] hash)
        {
            _value = Base58Encoding.Encode(_prefix, hash);
        }

        public P2PKHAddressBase Convert(BlockchainCryptoFactory factory)
        {
            var addr = factory.GetP2PKHAddress();
            addr.FromPublicKeyHash(Hash);
            return addr;
        }

        public void FromWIF(string value)
        {
            var key = Secp256k1PrivateKey.FromWIF(_wifPrefix, value);
            FromPrivateKey(key);
        }

        public byte[] ToPublicKey()
        {
            if (PrivateKey == null)
                throw new InvalidOperationException();

            if (PrivateKey.Compressed)
                return PrivateKey.ToCompressedPublicKey();
            else
                return PrivateKey.ToUncompressedPublicKey();
        }

        public void FromPrivateKey(Secp256k1PrivateKey key)
        {
            PrivateKey = key;
            FromPublicKey(ToPublicKey());
            IsCompressed = key.Compressed;
        }

        public string ToWIF()
        {
            if (PrivateKey == null)
                throw new InvalidOperationException();

            return PrivateKey.ToWIF(_wifPrefix);
        }

        public P2PKHAddressBase ToUncompressed()
        {
            if (IsCompressed == null)
                throw new InvalidOperationException();

            if (IsCompressed.Value)
            {
                var key = PrivateKey.Clone(false);
                var addr = _factory.GetP2PKHAddress();
                addr.FromPrivateKey(key);
                return addr;
            }
            else
                return this;
        }

        public P2PKHAddressBase ToCompressed()
        {
            if (IsCompressed == null)
                throw new InvalidOperationException();

            if (IsCompressed.Value)
                return this;
            else
            {
                var key = PrivateKey.Clone(true);
                var addr = _factory.GetP2PKHAddress();
                addr.FromPrivateKey(key);
                return addr;
            }
        }

        public override Script CreateRedeemScript()
        {
            var hash = Hash;
            if (hash?.Length != 20)
                throw new InvalidOperationException();

            var script = new Script();
            script.Code = new ScriptCode();
            script.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_DUP });
            script.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_HASH160 });
            script.Code.Add(new ScriptOpcodes.Operator { Data = hash });
            script.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_EQUALVERIFY });
            script.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_CHECKSIG });

            return script;
        }

        public override void Sign(TxInputBase input)
        {
            var tx = input.Transaction;
            if (tx == null)
                throw new InvalidOperationException();

            if (PrivateKey == null)
                throw new InvalidOperationException();

            byte[] pubKey = ToPublicKey();
            if (pubKey == null)
                throw new InvalidOperationException();

            byte[] hash = Hash;
            if (hash?.Length != 20)
                throw new InvalidOperationException();

            byte[] sigHash = input.GetSigHash(SignatureType.SIGHASH_ALL);
            byte[] signature = PrivateKey.Sign(sigHash);

            signature = ByteArray.Concat(signature, new byte[] { 0x01 });

            if (tx.IsSegWit)
            {
                input.SegWit = new[] { signature, pubKey };
                input.ScriptRaw = new byte[0];
            }
            else
            {
                input.Script = new Script();
                input.Script.Code = new ScriptCode();
                input.Script.Code.Add(new ScriptOpcodes.Operator { Data = signature });
                input.Script.Code.Add(new ScriptOpcodes.Operator { Data = pubKey });
                input.ScriptRaw = input.Script.ToBytes();
            }
        }
    }
}