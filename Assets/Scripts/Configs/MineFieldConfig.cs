using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "MineFieldConfig", menuName = "Configs/MineFieldConfig")]
    public class MineFieldConfig : ScriptableObject
    {
        [field: SerializeField, Min(1)] public int Rows { get; private set; }
        [field: SerializeField, Min(1)] public int Columns { get; private set; }
        [field: SerializeField, Min(0)] public int MinesCount { get; private set; }
    }
}