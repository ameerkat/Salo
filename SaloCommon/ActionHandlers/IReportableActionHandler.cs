namespace Salo
{
    /// <summary>
    /// Provides an interface for the simulation loop to update the report object for the player in the action handler
    /// itself. Allowing the action handler to update both the main game state and the player report preventing
    /// any sort of out of synch problems.
    /// 
    /// Doing this remotely the set report would have to actually call into the server  to get the results
    /// </summary>
    public interface IReportableActionHandler : IActionHandler
    {
        void SetReport(Report report);
    }
}
