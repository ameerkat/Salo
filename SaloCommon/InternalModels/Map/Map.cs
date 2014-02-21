using System;
using System.Collections.Generic;

namespace SaloCommon.InternalModels.Map
{
    /// <summary>
    /// In reality map is a list of stars, but since we don't want to serialize all the player
    /// information and that makes things more difficult to generate we define a map which is a list of
    /// simplified stars.
    /// </summary>
    public class Map
    {
        public List<MapStar> stars { get; set; }
    }
}
