namespace Salo
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class BotDescription : System.Attribute
    {
        private readonly string _description;
        public string Description { get { return _description; } }
        public BotDescription(string description)
        {
            this._description = description;
        }
    }
}
