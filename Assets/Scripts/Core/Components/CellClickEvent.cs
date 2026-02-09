using UnityEngine;

namespace Core.Components
{
    public enum CellClickButton : byte
    {
        Left = 0,
        Right = 1
    }

    public struct ClickCellRequest
    {
        public Vector2Int Position;
    }

    public struct OpenCellCommand
    {
        public int CellEntity;
    }

    public struct ToggleFlagRequest
    {
        public Vector2Int Position;
    }
}