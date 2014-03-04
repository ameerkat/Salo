namespace Salo
{
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class BotParameter : System.Attribute
    {
        private readonly string _name;
        public string Name { get { return _name; } }
        public BotParameter(string name)
        {
            this._name = name;
        }
    }
}
