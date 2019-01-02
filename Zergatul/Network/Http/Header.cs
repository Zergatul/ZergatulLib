namespace Zergatul.Network.Http
{
    public class Header
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public Header(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override string ToString() => $"{Name}: {Value}";
    }
}