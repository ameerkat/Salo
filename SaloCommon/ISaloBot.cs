namespace Salo
{
    public interface ISaloBot
    {
        void Initialize(Player player, Configuration configuration, IActionHandler actionHandler);
        void Run(Report game);
    }
}
