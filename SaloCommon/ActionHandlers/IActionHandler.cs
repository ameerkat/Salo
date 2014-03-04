namespace Salo
{
    public interface IActionHandler
    {
        void Upgrade(int starId, string upgrade);
        void BuildFleet(int starId);
        void Move(int originStarId, int destinationStarId, int ships);
        void SetCurrentResearch(string research);
        void SetNextResearch(string research);
    }
}
