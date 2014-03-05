namespace Salo
{
    /// <summary>
    /// Stateful action handler, has methods to manage the state of the game
    /// </summary>
    public interface IStatefulActionHandler : IActionHandler
    {
        void UpdateState(State state);
    }
}
