using System.Collections.Generic;

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
    }
}