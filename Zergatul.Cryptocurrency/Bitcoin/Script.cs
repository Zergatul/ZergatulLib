using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptocurrency.Bitcoin.ScriptOpcodes;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class Script
    {
        public List<Operator> Code { get; set; }

        public static Script FromBytes(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();

            var script = new Script();
            script.Code = new List<Operator>();

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

        public bool Run(params byte[][] stack)
        {
            return false;
        }
    }
}