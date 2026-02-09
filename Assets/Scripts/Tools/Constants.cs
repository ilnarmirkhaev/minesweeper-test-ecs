using UnityEngine;

namespace Tools
{
    public class Constants
    {
        public static readonly Vector2Int[] NeighborOffsets =
        {
            new(-1, -1), new(0, -1), new(1, -1),
            new(-1,  0),             new(1,  0),
            new(-1,  1), new(0,  1), new(1,  1)
        };
    }
}