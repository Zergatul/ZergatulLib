using System;
using System.Collections.Generic;

namespace Zergatul.Cryptocurrency
{
    public abstract class TxOutputBase
    {
        public ulong Amount { get; set; }
        public byte[] ScriptRaw { get; set; }

        public Script Script { get; set; }
        public AddressBase Address { get; set; }

        protected BlockchainCryptoFactory _factory;

        protected TxOutputBase(BlockchainCryptoFactory factory)
        {
            this._factory = factory;
        }

        public void Parse(byte[] data, ref int index)
        {
            if (data.Length < index + 8)
                throw new ArgumentOutOfRangeException();
            Amount = BitHelper.ToUInt64(data, index, ByteOrder.LittleEndian);
            index += 8;

            int scriptLength = checked((int)VarLengthInt.Parse(data, ref index));
            if (data.Length < index + scriptLength)
                throw new ArgumentOutOfRangeException();
            ScriptRaw = ByteArray.SubArray(data, index, scriptLength);
            index += scriptLength;

            // Parse script
            try
            {
                Script = Script.FromBytes(ScriptRaw);
            }
            catch (ScriptParseException)
            {
                Script = null;
            }
            if (Script?.IsPayToPublicKeyHash == true)
            {
                var addr = _factory.GetP2PKHAddress();
                addr.FromPublicKeyHash(Script.Code[2].Data);
                Address = addr;
            }
            if (Script?.IsPayToPublicKey == true)
            {
                var addr = _factory.GetP2PKAddress();
                addr.FromPublicKey(Script.Code[0].Data);
                Address = addr;
            }
            if (Script?.IsPayToScriptHash == true)
            {
                var addr = _factory.GetP2SHAddress();
                addr.FromScriptHash(Script.Code[1].Data);
                Address = addr;
            }
        }

        public void CreateScript()
        {
            if (Address == null)
                throw new InvalidOperationException();
            if (Address is P2PKHAddressBase)
            {
                var p2pkh = Address as P2PKHAddressBase;
                if (p2pkh.Hash?.Length != 20)
                    throw new InvalidOperationException();

                Script = new Script();
                Script.Code = new ScriptCode();
                Script.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_DUP });
                Script.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_HASH160 });
                Script.Code.Add(new ScriptOpcodes.Operator { Data = p2pkh.Hash });
                Script.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_EQUALVERIFY });
                Script.Code.Add(new ScriptOpcodes.Operator { Opcode = ScriptOpcodes.Opcode.OP_CHECKSIG });

                ScriptRaw = Script.ToBytes();
            }
            else
                throw new NotImplementedException();
        }

        public void Serialize(List<byte> bytes)
        {
            bytes.AddRange(BitHelper.GetBytes(Amount, ByteOrder.LittleEndian));
            bytes.AddRange(VarLengthInt.Serialize(ScriptRaw.Length));
            bytes.AddRange(ScriptRaw);
        }
    }
}