namespace Zergatul.Cryptography.Certificate
{
    public class AttributePair<TKey, TValue>
    {
        public TKey Key { get; private set; }
        public TValue Value { get; private set; }

        public AttributePair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}