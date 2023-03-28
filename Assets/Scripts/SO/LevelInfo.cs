using UnityEngine;

namespace DungeonDraws.Scripts.Systems.LevelGeneration
{
    [CreateAssetMenu(fileName = "New Level", menuName = "ScriptableObject/LevelData")]
    public class LevelData: ScriptableObject
    {
        public int _mapHeight = 100;
        public int _mapWidth = 100;
        public int _roomsNumberMin = 5;
        public int _roomsNumberMax = 15;
        public int _roomSizeMin = 5;
        public int _roomSizeMax = 20;
        public int _corridorLengthMin = 2;
        public int _corridorLengthMax = 7;
        public int _corridorWidthMin = 3;
        public int _corridorWidthMax = 3;
        public int _seed = 123456;
    }
}