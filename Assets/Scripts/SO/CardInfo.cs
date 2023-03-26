using DungeonDraws.Game;
using UnityEngine;

namespace DungeonDraws.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/CardInfo", fileName = "CardInfo")]
    public class CardInfo : ScriptableObject
    {
        public string Name;
        [Multiline]
        public string Description;
        public Sprite Image;
        public Modifier[] Modifiers;
    }
}