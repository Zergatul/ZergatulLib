using System;

namespace Zergatul.Cryptocurrency.Base
{
    /// <summary>
    /// Pay-to-script-hash
    /// </summary>
    public abstract class P2SHAddressBase : Base58AddressBase
    {
        protected BlockchainCryptoFactory _factory;

        protected P2SHAddressBase(BlockchainCryptoFactory factory)
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

        public void FromScript(byte[] scriptData)
        {
            FromScriptHash(RIPE160SHA256.Hash(scriptData));
        }

        public void FromScriptHash(byte[] hash)
        {
            _value = Base58Encoding.Encode(_prefix, hash);
        }

        public P2SHAddressBase Convert(BlockchainCryptoFactory factory)
        {
            var addr = factory.GetP2SHAddress();
            addr.FromScriptHash(Hash);
            return addr;
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
            throw new NotImplementedException();
        }
    }
}