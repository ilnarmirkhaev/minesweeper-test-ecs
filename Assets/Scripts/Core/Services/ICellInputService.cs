using UnityEngine;
using Core.Components;

namespace Core.Services
{
    public interface ICellInputService
    {
        void ClickCell(Vector2Int position, CellClickButton button);
        void RequestRestart();
    }
}

