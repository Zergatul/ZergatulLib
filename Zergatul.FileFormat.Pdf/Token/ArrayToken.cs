using System.Collections.Generic;

namespace Zergatul.FileFormat.Pdf.Token
{
    internal class ArrayToken : TokenBase
    {
        public override bool IsBasic => true;
        public IReadOnlyList<TokenBase> Value { get; }

        public ArrayToken(List<TokenBase> value)
        {
            Value = value;
        }
    }
}