using UnityEngine;

namespace DungeonDraws.Scripts.Systems.LevelGeneration
{
    [CreateAssetMenu(fileName = "New Level Style", menuName = "ScriptableObject/LevelStyle")]
    public class LevelStyle: ScriptableObject
    {
        public GameObject _floorPrefab;
        public GameObject _wallPrefab;
        public GameObject _wallSeparatorPrefab;
        public GameObject _cornerInPrefab;
        public GameObject _cornerOutPrefab;
    }
}