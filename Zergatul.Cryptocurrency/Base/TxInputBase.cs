using System;
using System.Collections.Generic;
using System.Linq;
using Zergatul.Security;

namespace Zergatul.Cryptocurrency.Base
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

        private TxOutputBase _prevOutput;
        public TxOutputBase PrevOutput
        {
            get
            {
                return _prevOutput;
            }
            internal set
            {
                _prevOutput = value;
                if (value != null)
                    Amount = value.Amount;
                else
                    Amount = null;
            }
        }

        public ulong? Amount { get; private set; }

        internal Script PrevOutputExpandedScript { get; set; }

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
        }

        internal void ParseAddress()
        {
            if (Script?.IsPayToPublicKeyHashInput == true)
            {
                var addr = _factory.GetP2PKHAddress();
                addr.FromPublicKey(Script.Code[1].Data);
                Address = addr;
            }
            if (Script?.IsPayToWitnessPublicKeyHashInput == true)
            {
                bool segwitOk =
                   SegWit != null &&
                   SegWit.Length == 2 &&
                   SegWit[0].Length == 71 &&
                   SegWit[1].Length == 33;
                if (segwitOk)
                {
                    var addr = _factory.GetP2WPKHAddress();
                    addr.FromPublicKey(SegWit[1]);
                    Address = addr;
                }
            }
            if (Script?.IsPayToScriptHashPayToWitnessPublicKeyHashInput == true)
            {
                bool segwitOk =
                    SegWit != null &&
                    SegWit.Length == 2 &&
                    SegWit[0].Length == 71 &&
                    SegWit[1].Length == 33;
                if (segwitOk)
                {
                    var addr = _factory.GetP2SHP2WPKHAddress();
                    addr.FromScript(Script.Code[0].Data);
                    Address = addr;
                }
            }
        }

        public bool Verify()
        {
            if (PrevTransaction == null)
                return false;

            PrevOutput = PrevTransaction.GetOutputs().ElementAt(PrevTxOutIndex);

            if (PrevOutput == null)
                return false;

            if (PrevOutput.Script == null)
                return false;

            if (Script == null)
                return false;

            if (Script.Code.Any(o => o.Data == null))
                return false;

            return PrevOutput.Script.Run(this);
        }

        public virtual byte[] GetSigHash(int hashType)
        {
            if (Transaction == null)
                throw new InvalidOperationException();

            if (!Transaction.IsSegWit)
            {
                if ((hashType & 0x31) == SignatureType.SIGHASH_NONE)
                    throw new NotImplementedException();
                else if ((hashType & 0x31) == SignatureType.SIGHASH_SINGLE)
                    throw new NotImplementedException();
                else if ((hashType & SignatureType.SIGHASH_ANYONECANPAY) != 0)
                    throw new NotImplementedException();
                else if ((hashType & SignatureType.SIGHASH_ALL) != 0)
                {
                    List<byte> txCopy = new List<byte>();
                    Transaction.SerializeHeader(txCopy);

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

                if ((hashType & SignatureType.SIGHASH_ANYONECANPAY) == 0)
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

                if ((hashType & SignatureType.SIGHASH_ANYONECANPAY) == 0 && (hashType & 0x1f) != SignatureType.SIGHASH_SINGLE && (hashType & 0x1f) != SignatureType.SIGHASH_NONE)
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

                if ((hashType & 0x1f) != SignatureType.SIGHASH_SINGLE && (hashType & 0x1f) != SignatureType.SIGHASH_NONE)
                {
                    List<byte> buffer = new List<byte>();
                    foreach (var output in Transaction.GetOutputs())
                    {
                        output.Serialize(buffer);
                    }
                    hashOutputs = DoubleSHA256.Hash(buffer.ToArray());
                }
                else if ((hashType & 0x1f) == SignatureType.SIGHASH_SINGLE /*&& nIn < txTo.vout.size()*/)
                {
                    throw new NotImplementedException();
                    //CHashWriter ss(SER_GETHASH, 0);
                    //ss << txTo.vout[nIn];
                    //hashOutputs = ss.GetHash();
                }
                else
                    hashOutputs = new byte[32];

                var prevOutput = PrevTransaction.GetOutputs().ElementAt(PrevTxOutIndex);

                using (var sha256 = SecurityProvider.GetMessageDigestInstance(MessageDigests.SHA256))
                {
                    sha256.Update(BitHelper.GetBytes(Transaction.Version, ByteOrder.LittleEndian));

                    sha256.Update(hashPrevOuts);
                    sha256.Update(hashSequence);

                    sha256.Update(PrevTx.Reverse().ToArray());
                    sha256.Update(BitHelper.GetBytes(PrevTxOutIndex, ByteOrder.LittleEndian));

                    if (PrevOutputExpandedScript == null)
                        throw new InvalidOperationException();

                    byte[] raw = PrevOutputExpandedScript.Raw ?? PrevOutputExpandedScript.ToBytes();
                    sha256.Update(VarLengthInt.Serialize(raw.Length));
                    sha256.Update(raw);

                    sha256.Update(BitHelper.GetBytes(prevOutput.Amount, ByteOrder.LittleEndian));

                    sha256.Update(BitHelper.GetBytes(SequenceNo, ByteOrder.LittleEndian));
                    sha256.Update(hashOutputs);
                    sha256.Update(BitHelper.GetBytes(Transaction.LockTime, ByteOrder.LittleEndian));
                    sha256.Update(BitHelper.GetBytes(hashType, ByteOrder.LittleEndian));

                    using (var sha256inner = SecurityProvider.GetMessageDigestInstance(MessageDigests.SHA256))
                        return sha256inner.Digest(sha256.Digest());
                }
            }
        }

        public void Serialize(List<byte> buffer)
        {
            buffer.AddRange(PrevTx.Reverse());
            buffer.AddRange(BitHelper.GetBytes(PrevTxOutIndex, ByteOrder.LittleEndian));
            buffer.AddRange(VarLengthInt.Serialize(ScriptRaw.Length));
            buffer.AddRange(ScriptRaw);
            buffer.AddRange(BitHelper.GetBytes(SequenceNo, ByteOrder.LittleEndian));
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