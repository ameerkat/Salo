namespace Salo
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class BotDescription : System.Attribute
    {
        private string _description;
        public BotDescription(string description)
        {
            this._description = description;
        }
    }
}
