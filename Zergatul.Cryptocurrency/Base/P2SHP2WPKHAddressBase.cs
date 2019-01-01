using System;
using Zergatul.Security;

namespace Zergatul.Cryptocurrency.Base
{
    public abstract class P2SHP2WPKHAddressBase : Base58AddressBase
    {
        public byte[] PublicKey => _publicKey;

        private byte[] _privateKey;
        private byte[] _publicKey;
        protected byte[] _keyHash;

        public void Parse(string address)
        {
            throw new NotImplementedException();
        }

        public void FromPrivateKey(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (key.Length != 32)
                throw new InvalidOperationException();

            _privateKey = key;

            using (var generator = SecurityProvider.GetKeyPairGeneratorInstance(KeyPairGenerators.EC))
            {
                generator.Init(new ECKeyPairGeneratorParameters { Curve = Curves.secp256k1 });
                var publicKey = generator.GetPublicKey(new RawPrivateKey(key));
                FromPublicKey(generator.Format(publicKey, KeyFormat.Compressed));
            }
        }

        public void FromPublicKey(byte[] pubkeyData)
        {
            if (pubkeyData == null)
                throw new ArgumentNullException(nameof(pubkeyData));
            if (pubkeyData.Length != 33)
                throw new InvalidOperationException();
            if (pubkeyData[0] != 0x02 && pubkeyData[0] != 0x03)
                throw new InvalidOperationException();

            _publicKey = pubkeyData;
            _keyHash = RIPE160SHA256.Hash(pubkeyData);

            DeriveFromKeyHash();
        }

        public void FromPublicKeyHash(byte[] hash)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash));
            if (hash.Length != 20)
                throw new InvalidOperationException();

            _keyHash = hash;
            DeriveFromKeyHash();
        }

        public void FromScript(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length != 22)
                throw new InvalidOperationException();
            if (data[0] != 0x00 || data[1] != 0x14)
                throw new InvalidOperationException();

            _value = Base58Encoding.Encode(_prefix, RIPE160SHA256.Hash(data));
        }

        private void DeriveFromKeyHash()
        {
            if (_keyHash == null || _keyHash.Length != 20)
                throw new InvalidOperationException();

            _value = Base58Encoding.Encode(_prefix, RIPE160SHA256.Hash(new byte[] { 0x00, 0x14 }, _keyHash));
        }

        public override Script CreateRedeemScript()
        {
            var hash = Hash;
            if (hash?.Length != 20)
                throw new InvalidOperationException();

            var script = new Script();
            script.Code = new ScriptCode();
            script.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_HASH160 });
            script.Code.Add(new ScriptOpcodes.Operator { Data = hash });
            script.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_EQUAL });

            return script;
        }

        public override void Sign(TxInputBase input)
        {
            var tx = input.Transaction;
            if (tx == null)
                throw new InvalidOperationException();

            if (!tx.IsSegWit)
                throw new InvalidOperationException();

            if (_privateKey == null)
                throw new InvalidOperationException();

            if (_publicKey == null)
                throw new InvalidOperationException();

            if (_keyHash == null || _keyHash.Length != 20)
                throw new InvalidOperationException();

            byte[] hash = Hash;
            if (hash?.Length != 20)
                throw new InvalidOperationException();

            input.PrevOutputExpandedScript = new Script();
            input.PrevOutputExpandedScript.Code = new ScriptCode();
            input.PrevOutputExpandedScript.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_DUP });
            input.PrevOutputExpandedScript.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_HASH160 });
            input.PrevOutputExpandedScript.Code.Add(new ScriptOpcodes.Operator { Data = _keyHash });
            input.PrevOutputExpandedScript.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_EQUALVERIFY });
            input.PrevOutputExpandedScript.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_CHECKSIG });

            byte[] sigHash = input.GetSigHash(SignatureType.SIGHASH_ALL);
            byte[] signature;
            using (var ecdsa = Signature.GetInstance(Signatures.ECDSA))
            {
                ecdsa.InitForSign(new RawPrivateKey(_privateKey), new ECDSASignatureParameters
                {
                    Curve = Curves.secp256k1,
                    Random = SecureRandom.GetInstance(SecureRandoms.Default),
                    LowS = true
                });
                ecdsa.Update(sigHash, 0, sigHash.Length);
                signature = ecdsa.Sign();
            }

            signature = ByteArray.Concat(signature, new byte[] { 0x01 });

            input.SegWit = new[] { signature, _publicKey };
            input.Script = new Script();
            input.Script.Code = new ScriptCode();
            input.Script.Code.Add(new ScriptOpcodes.Operator
            {
                Data = ByteArray.Concat(new byte[] { 0x00, 0x14 }, _keyHash)
            });
            input.ScriptRaw = input.Script.ToBytes();
        }
    }
}