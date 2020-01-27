namespace Zergatul.FileFormat.Pdf.Token
{
    internal class BooleanToken : TokenBase
    {
        public static BooleanToken False { get; } = new BooleanToken(false);
        public static BooleanToken True { get; } = new BooleanToken(true);

        public bool Value { get; }

        private BooleanToken(bool value)
        {
            Value = value;
        }
    }
}