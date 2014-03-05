using Salo.Live.Models;

namespace Salo
{
    public static class Extensions
    {
        public static Player Player(this State state, Star star)
        {
            return state.Players[star.PlayerId];
        }

        public static Player Player(this State state, Fleet fleet)
        {
            return state.Players[fleet.PlayerId];
        }
    }
}
