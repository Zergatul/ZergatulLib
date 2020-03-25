using System.Collections.Generic;
using System.Linq;

namespace Zergatul.FileFormat.Pdf.Token
{
    internal class HexStringToken : TokenBase
    {
        public override bool IsBasic => true;
        public IReadOnlyList<byte> Value { get; }

        public HexStringToken(IReadOnlyList<byte> value)
        {
            Value = value;
        }

#if DEBUG
        public override string ToString()
        {
            return "<" + BitHelper.BytesToHex(Value.ToArray()) + ">";
        }
#endif
    }
}