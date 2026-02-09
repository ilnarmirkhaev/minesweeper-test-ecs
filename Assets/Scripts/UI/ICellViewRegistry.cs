using UnityEngine;

namespace UI
{
    public interface ICellViewRegistry
    {
        void Register(Vector2Int position, CellView view);
        bool TryGet(Vector2Int position, out CellView view);
        void Clear();
    }
}

