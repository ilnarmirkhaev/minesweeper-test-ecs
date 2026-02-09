using Configs;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI
{
    public sealed class FieldView : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private CellView _cellTemplate;

        [Inject] private MineFieldConfig _config;
        [Inject] private ICellViewRegistry _registry;
        [Inject] private IObjectResolver _resolver;

        private void Awake()
        {
            if (_container == null)
                _container = transform as RectTransform;
        }

        private void Start()
        {
            if (_config == null || _registry == null)
                return;

            if (_cellTemplate == null)
                return;

            _registry.Clear();

            _cellTemplate.gameObject.SetActive(false);

            for (var row = 0; row < _config.Rows; row++)
            {
                for (var col = 0; col < _config.Columns; col++)
                {
                    // TODO: move to factory?
                    var pos = new Vector2Int(row, col);
                    var view = _resolver.Instantiate(_cellTemplate, _container);
                    view.gameObject.name = $"Cell_{row}_{col}";
                    view.gameObject.SetActive(true);
                    view.Initialize(pos);
                    _registry.Register(pos, view);
                }
            }
        }
    }
}