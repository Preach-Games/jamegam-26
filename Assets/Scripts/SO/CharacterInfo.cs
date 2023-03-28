using DungeonDraws.Character;
using UnityEngine;

namespace DungeonDraws.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/CharacterInfo", fileName = "CharacterInfo")]
    public class CharacterInfo : ScriptableObject
    {
        public int Physique;
        public int Agility;
        public int Mind;

        public GameObject Prefab;
        public Faction Faction;
        public Race Race;
    }
}