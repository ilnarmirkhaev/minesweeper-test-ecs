using Configs;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace UI
{
    public sealed class FieldView : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private CellView _cellTemplate;
        [SerializeField] private GridLayoutGroup _grid;

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

            ValidateGrid();
            _registry.Clear();

            _cellTemplate.gameObject.SetActive(false);

            for (var row = 0; row < _config.Rows; row++)
            {
                for (var col = 0; col < _config.Columns; col++)
                {
                    CreateCellView(row, col);
                }
            }
        }

        private void CreateCellView(int row, int col)
        {
            var pos = new Vector2Int(row, col);
            var view = _resolver.Instantiate(_cellTemplate, _container);
            view.gameObject.name = $"Cell_{row}_{col}";
            view.gameObject.SetActive(true);
            view.Initialize(pos);
            _registry.Register(pos, view);
        }

        private void ValidateGrid()
        {
            _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _grid.constraintCount = _config.Columns;
        }
    }
}