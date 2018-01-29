using System;
using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Cryptocurrency
{
    public abstract class TxInputBase
    {
        public byte[] PrevTx { get; set; }
        public int PrevTxOutIndex { get; set; }
        public byte[] ScriptRaw { get; set; }
        public uint SequenceNo { get; set; }
        public byte[][] SegWit { get; set; }

        public string PrevTxIDString => BitHelper.BytesToHex(PrevTx);
        public Script Script { get; set; }
        public AddressBase Address { get; set; }

        public TransactionBase Transaction { get; set; }
        public TransactionBase PrevTransaction { get; set; }

        protected BlockchainCryptoFactory _factory;

        protected TxInputBase(BlockchainCryptoFactory factory)
        {
            this._factory = factory;
        }

        public void Parse(byte[] data, ref int index)
        {
            if (data.Length < index + 32)
                throw new ArgumentOutOfRangeException();
            PrevTx = ByteArray.SubArray(data, index, 32);
            Array.Reverse(PrevTx);
            index += 32;

            if (data.Length < index + 4)
                throw new ArgumentOutOfRangeException();
            PrevTxOutIndex = BitHelper.ToInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            int scriptLength = checked((int)VarLengthInt.Parse(data, ref index));
            if (data.Length < index + scriptLength)
                throw new ArgumentOutOfRangeException();
            ScriptRaw = ByteArray.SubArray(data, index, scriptLength);
            index += scriptLength;

            if (data.Length < index + 4)
                throw new ArgumentOutOfRangeException();
            SequenceNo = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            // Parse script
            try
            {
                Script = Script.FromBytes(ScriptRaw);
            }
            catch (ScriptParseException)
            {
                Script = null;
            }
            if (Script?.IsPayToPublicKeyHashInput ?? false)
            {
                var addr = _factory.GetP2PKHAddress();
                addr.FromPublicKey(Script.Code[1].Data);
                Address = addr;
            }
        }

        public bool Verify()
        {
            if (PrevTransaction == null)
                return false;

            if (PrevTxOutIndex >= PrevTransaction.GetOutputs().Count())
                return false;

            var prevOutput = PrevTransaction.GetOutputs().ElementAt(PrevTxOutIndex);
            var prevOutScript = prevOutput.Script;

            if (prevOutScript == null)
                return false;

            if (Script == null)
                return false;

            if (Script.Code.Any(o => o.Data == null))
                return false;

            return prevOutScript.Run(this);
        }

        private const int SIGHASH_ALL = 0x00000001;
        private const int SIGHASH_NONE = 0x00000002;
        private const int SIGHASH_SINGLE = 0x00000003;
        private const int SIGHASH_ANYONECANPAY = 0x00000080;

        public virtual byte[] GetSigHash(int hashType)
        {
            if (Transaction == null)
                throw new InvalidOperationException();

            if (!Transaction.IsSegWit)
            {
                if ((hashType & 0x31) == SIGHASH_NONE)
                    throw new NotImplementedException();
                else if ((hashType & 0x31) == SIGHASH_SINGLE)
                    throw new NotImplementedException();
                else if ((hashType & SIGHASH_ANYONECANPAY) != 0)
                    throw new NotImplementedException();
                else if ((hashType & SIGHASH_ALL) != 0)
                {
                    List<byte> txCopy = new List<byte>();

                    txCopy.AddRange(BitHelper.GetBytes(Transaction.Version, ByteOrder.LittleEndian));

                    txCopy.AddRange(VarLengthInt.Serialize(Transaction.GetInputs().Count()));
                    foreach (var input in Transaction.GetInputs())
                        if (input != this)
                            input.SerializeWithoutScripts(txCopy);
                        else
                            input.SerializeWithScript(txCopy, input.PrevTransaction.GetOutputs().ElementAt(PrevTxOutIndex).ScriptRaw);

                    txCopy.AddRange(VarLengthInt.Serialize(Transaction.GetOutputs().Count()));
                    foreach (var output in Transaction.GetOutputs())
                        output.Serialize(txCopy);

                    txCopy.AddRange(BitHelper.GetBytes(Transaction.LockTime, ByteOrder.LittleEndian));
                    txCopy.AddRange(BitHelper.GetBytes(hashType, ByteOrder.LittleEndian));

                    return DoubleSHA256.Hash(txCopy.ToArray());
                }
                else
                    throw new InvalidOperationException();
            }
            else
            {
                // https://www.facebook.com/wasiliyivanov/posts/10204829512857338
                // https://github.com/bitcoin/bips/blob/master/bip-0143.mediawiki
                // http://n.bitcoin.ninja/checktx?txid=8139979112e894a14f8370438a471d23984061ff83a9eba0bc7a34433327ec21

                byte[] hashPrevOuts = null;
                byte[] hashSequence = null;
                byte[] hashOutputs = null;

                if ((hashType & SIGHASH_ANYONECANPAY) == 0)
                {
                    List<byte> buffer = new List<byte>();
                    foreach (var input in Transaction.GetInputs())
                    {
                        buffer.AddRange(input.PrevTx.Reverse());
                        buffer.AddRange(BitHelper.GetBytes(input.PrevTxOutIndex, ByteOrder.LittleEndian));
                    }
                    hashPrevOuts = DoubleSHA256.Hash(buffer.ToArray());
                }
                else
                    hashPrevOuts = new byte[32];

                if ((hashType & SIGHASH_ANYONECANPAY) == 0 && (hashType & 0x1f) != SIGHASH_SINGLE && (hashType & 0x1f) != SIGHASH_NONE)
                {
                    List<byte> buffer = new List<byte>();
                    foreach (var input in Transaction.GetInputs())
                    {
                        buffer.AddRange(BitHelper.GetBytes(input.SequenceNo, ByteOrder.LittleEndian));
                    }
                    hashSequence = DoubleSHA256.Hash(buffer.ToArray());
                }
                else
                    hashSequence = new byte[32];

                if ((hashType & 0x1f) != SIGHASH_SINGLE && (hashType & 0x1f) != SIGHASH_NONE)
                {
                    List<byte> buffer = new List<byte>();
                    foreach (var output in Transaction.GetOutputs())
                    {
                        output.Serialize(buffer);
                    }
                    hashOutputs = DoubleSHA256.Hash(buffer.ToArray());
                }
                else if ((hashType & 0x1f) == SIGHASH_SINGLE /*&& nIn < txTo.vout.size()*/)
                {
                    throw new NotImplementedException();
                    //CHashWriter ss(SER_GETHASH, 0);
                    //ss << txTo.vout[nIn];
                    //hashOutputs = ss.GetHash();
                }
                else
                    hashOutputs = new byte[32];

                var prevOutput = PrevTransaction.GetOutputs().ElementAt(PrevTxOutIndex);

                var dsha256 = new DoubleSHA256();

                dsha256.Update(BitHelper.GetBytes(Transaction.Version, ByteOrder.LittleEndian));

                dsha256.Update(hashPrevOuts);
                dsha256.Update(hashSequence);

                dsha256.Update(PrevTx.Reverse().ToArray());
                dsha256.Update(BitHelper.GetBytes(PrevTxOutIndex, ByteOrder.LittleEndian));

                byte[] raw = prevOutput.Script.Expand().Raw;
                dsha256.Update(VarLengthInt.Serialize(raw.Length));
                dsha256.Update(raw);

                dsha256.Update(BitHelper.GetBytes(prevOutput.Amount, ByteOrder.LittleEndian));

                dsha256.Update(BitHelper.GetBytes(SequenceNo, ByteOrder.LittleEndian));
                dsha256.Update(hashOutputs);
                dsha256.Update(BitHelper.GetBytes(Transaction.LockTime, ByteOrder.LittleEndian));
                dsha256.Update(BitHelper.GetBytes(hashType, ByteOrder.LittleEndian));

                return dsha256.ComputeHash();
            }
        }

        public void SerializeWithoutScripts(List<byte> bytes)
        {
            bytes.AddRange(PrevTx.Reverse());
            bytes.AddRange(BitHelper.GetBytes(PrevTxOutIndex, ByteOrder.LittleEndian));
            bytes.Add(0); // script length
            bytes.AddRange(BitHelper.GetBytes(SequenceNo, ByteOrder.LittleEndian));
        }

        public void SerializeWithScript(List<byte> bytes, byte[] script)
        {
            bytes.AddRange(PrevTx.Reverse());
            bytes.AddRange(BitHelper.GetBytes(PrevTxOutIndex, ByteOrder.LittleEndian));
            bytes.AddRange(VarLengthInt.Serialize(script.Length));
            bytes.AddRange(script);
            bytes.AddRange(BitHelper.GetBytes(SequenceNo, ByteOrder.LittleEndian));
        }
    }
}