using System.Collections.Generic;
using System.Linq;

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

        public bool Is<T>(int length)
            where T : TokenBase
        {
            if (Value.Count != length)
                return false;
            for (int i = 0; i < length; i++)
                if (!(Value[i] is T))
                    return false;
            return true;
        }

        public bool ValidateType<T>()
            where T : TokenBase
        {
            foreach (var token in Value)
                if (token as T == null)
                    return false;
            return true;
        }

#if DEBUG
        public override string ToString()
        {
            return "[ " + string.Join(", ", Value.Select(t => t.ToString())) + " ]";
        }
#endif
    }
}