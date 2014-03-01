namespace Salo
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class BotName : System.Attribute
    {
        private readonly string _name;
        private readonly string _version;

        public string Name { get { return _name; } }
        public string Version { get { return _version; } }
        public BotName(string name, string version)
        {
            this._name = name;
            this._version = version;
        }
    }
}
