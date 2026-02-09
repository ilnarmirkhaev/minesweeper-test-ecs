using UnityEngine;

namespace Core.Services
{
    public interface ICellLookup
    {
        bool TryGetCellEntity(Vector2Int position, out int entity);
    }
}

