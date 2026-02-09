using UnityEngine;

namespace Core.Components
{
    public enum CellClickButton : byte
    {
        Left = 0,
        Right = 1
    }

    public struct OpenCellRequest
    {
        public Vector2Int Position;
    }

    public struct ToggleFlagRequest
    {
        public Vector2Int Position;
    }
}

