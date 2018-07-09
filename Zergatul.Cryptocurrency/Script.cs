using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptocurrency.ScriptOpcodes;

namespace Zergatul.Cryptocurrency
{
    public class Script
    {
        public ScriptCode Code { get; set; }
        public byte[] Raw { get; set; }

        public static Script FromBytes(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();

            var script = new Script();
            script.Raw = data;
            script.Code = new ScriptCode();

            int index = 0;
            while (index < data.Length)
            {
                byte @byte = data[index++];
                if (1 <= @byte && @byte <= 0x4B)
                {
                    if (index + @byte > data.Length)
                        throw new ScriptParseException();
                    script.Code.Add(new Operator
                    {
                        Data = ByteArray.SubArray(data, index, @byte)
                    });
                    index += @byte;
                    continue;
                }

                Opcode opcode = (Opcode)@byte;

                int length = -1;
                switch (opcode)
                {
                    case Opcode.OP_PUSHDATA1:
                        if (data.Length > index + 1)
                            throw new ScriptParseException();
                        length = data[index++];
                        continue;
                    case Opcode.OP_PUSHDATA2:
                        if (data.Length > index + 2)
                            throw new ScriptParseException();
                        length = BitHelper.ToUInt16(data, index, ByteOrder.BigEndian);
                        index += 2;
                        continue;
                    case Opcode.OP_PUSHDATA4:
                        if (data.Length > index + 4)
                            throw new ScriptParseException();
                        length = BitHelper.ToInt32(data, index, ByteOrder.BigEndian);
                        index += 4;
                        continue;
                }
                if (length != -1)
                {
                    if (data.Length > index + length)
                        throw new ScriptParseException();
                    script.Code.Add(new Operator
                    {
                        Data = ByteArray.SubArray(data, index, length)
                    });
                    index += length;
                    continue;
                }

                script.Code.Add(new Operator
                {
                    Opcode = opcode
                });
            }

            return script;
        }

        public static Script FromHex(string hex) => FromBytes(BitHelper.HexToBytes(hex));

        public bool IsPayToPublicKeyHash =>
            Code != null &&
            Code.Count == 5 &&
            Code[0].Opcode == Opcode.OP_DUP &&
            Code[1].Opcode == Opcode.OP_HASH160 &&
            Code[2].Data?.Length == 20 &&
            Code[3].Opcode == Opcode.OP_EQUALVERIFY &&
            Code[4].Opcode == Opcode.OP_CHECKSIG;

        public bool IsPayToWitnessPublicKeyHash =>
            Code != null &&
            Code.Count == 2 &&
            Code[0].Opcode == Opcode.OP_0 &&
            Code[1].Data?.Length == 20;

        public bool IsPayToPublicKeyHashInput =>
            Code != null &&
            Code.Count == 2 &&
            (Code[0].Data?.Length == 71 || Code[0].Data?.Length == 72) &&
            (Code[1].Data?.Length == 33 || Code[1].Data?.Length == 65);

        public bool IsPayToPublicKey =>
            Code != null &&
            Code.Count == 2 &&
            (Code[0].Data?.Length == 33 || Code[0].Data?.Length == 65) &&
            Code[1].Opcode == Opcode.OP_CHECKSIG;

        public bool IsPayToScriptHash =>
            Code != null &&
            Code.Count == 3 &&
            Code[0].Opcode == Opcode.OP_HASH160 &&
            Code[1].Data?.Length == 20 &&
            Code[2].Opcode == Opcode.OP_EQUAL;

        public bool Run(TxInputBase input)
        {
            if (input.Script.Code.Any(o => o.Data == null))
                throw new InvalidOperationException();

            if (input.SegWit == null)
                return Run(input, input.Script.Code.Select(o => o.Data));
            else
                return Run(input, input.SegWit);
        }

        public bool Run(TxInputBase input, IEnumerable<byte[]> initialStack)
        {
            Stack<byte[]> stack = new Stack<byte[]>(initialStack);

            try
            {
                foreach (var op in Expand().Code)
                    if (!op.Run(input, stack))
                        return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            if (stack.Count == 0)
                return false;

            byte[] result = stack.Pop();
            for (int i = 0; i < result.Length; i++)
                if (result[i] != 0)
                {
                    // BIP-0016
                    if (IsPayToScriptHash)
                    {
                        int count = input.Script.Code.Count;
                        var serializedScript = FromBytes(input.Script.Code[count - 1].Data);
                        return serializedScript.Run(input, input.Script.Code.Take(count - 1).Select(o => o.Data));
                    }
                    else
                        return true;
                }

            return false;
        }

        public Script Expand()
        {
            if (IsPayToWitnessPublicKeyHash)
            {
                var script = new Script();
                script.Code = new ScriptCode
                {
                    new Operator { Opcode = Opcode.OP_DUP },
                    new Operator { Opcode = Opcode.OP_HASH160 },
                    new Operator { Data = Code[1].Data },
                    new Operator { Opcode = Opcode.OP_EQUALVERIFY },
                    new Operator { Opcode = Opcode.OP_CHECKSIG },
                };
                script.Raw = new byte[25];
                script.Raw[0] = (byte)Opcode.OP_DUP;
                script.Raw[1] = (byte)Opcode.OP_HASH160;
                script.Raw[2] = 20;
                Array.Copy(Code[1].Data, 0, script.Raw, 3, 20);
                script.Raw[23] = (byte)Opcode.OP_EQUALVERIFY;
                script.Raw[24] = (byte)Opcode.OP_CHECKSIG;
                return script;
            }
            else
                return this;
        }

        public byte[] ToBytes()
        {
            var list = new List<byte>();
            foreach (var op in Code)
            {
                if (op.Data == null && op.Opcode == null)
                    throw new InvalidOperationException();
                if (op.Data != null && op.Opcode != null)
                    throw new InvalidOperationException();

                if (op.Opcode != null)
                    list.Add((byte)op.Opcode.Value);

                if (op.Data != null)
                {
                    if (op.Data.Length >= 1 && op.Data.Length <= 0x4B)
                    {
                        list.Add((byte)op.Data.Length);
                        list.AddRange(op.Data);
                    }
                    else
                        throw new NotImplementedException();
                }
            }

            return list.ToArray();
        }
    }

    [System.Diagnostics.DebuggerDisplay("{CodeStr}")]
    public class ScriptCode : List<Operator>
    {
        public string CodeStr => string.Join(" ", this.Select(o => o.Data != null ? BitHelper.BytesToHex(o.Data) : o.Opcode.Value.ToString()));

        public override string ToString()
        {
            return CodeStr;
        }
    }
}