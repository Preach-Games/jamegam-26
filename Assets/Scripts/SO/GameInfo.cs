using UnityEngine;

namespace DungeonDraws.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameInfo", fileName = "GameInfo")]
    public class GameInfo : ScriptableObject
    {
        [Header("Card")]
        public int CardCount;
        public float TimeBeforeCardDisplay;
        public GameObject CardPrefab;
    }
}