using System;
using Core.Components;
using Core.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace UI
{
    public class CellView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _background;
        [SerializeField] private Text _neighborBombsText;

        [Inject] private ICellInputService _input;

        private Vector2Int _position;

        public void Initialize(Vector2Int position)
        {
            _position = position;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_input == null) return;

            if (eventData.button == PointerEventData.InputButton.Middle) return;

            var btn = eventData.button switch
            {
                PointerEventData.InputButton.Left => CellClickButton.Left,
                PointerEventData.InputButton.Right => CellClickButton.Right,
                _ => throw new ArgumentOutOfRangeException()
            };

            _input.ClickCell(_position, btn);
        }

        public void SetVisual(CellVisualState state, int? neighborBombsCount = null)
        {
            if (_background)
            {
                _background.color = state switch
                {
                    CellVisualState.Closed => new Color(0.65f, 0.65f, 0.65f),
                    CellVisualState.Opened => new Color(0.85f, 0.85f, 0.85f),
                    CellVisualState.Flagged => new Color(0.95f, 0.7f, 0.2f),
                    CellVisualState.Exploded => new Color(0.9f, 0.2f, 0.2f),
                    _ => _background.color
                };
            }

            if (_neighborBombsText)
            {
                var showNumber = state == CellVisualState.Opened && neighborBombsCount is > 0;
                _neighborBombsText.gameObject.SetActive(showNumber);
                _neighborBombsText.text = showNumber ? neighborBombsCount.Value.ToString() : "";
            }
        }
    }

    public enum CellVisualState : byte
    {
        Closed = 0,
        Opened = 1,
        Flagged = 2,
        Exploded = 3
    }
}