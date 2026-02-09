using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public sealed class CellViewRegistry : ICellViewRegistry
    {
        private readonly Dictionary<Vector2Int, CellView> _map = new();

        public void Register(Vector2Int position, CellView view) => _map[position] = view;

        public bool TryGet(Vector2Int position, out CellView view) => _map.TryGetValue(position, out view);

        public void Clear() => _map.Clear();
    }
}

