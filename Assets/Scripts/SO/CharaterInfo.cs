using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonDraws.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/CharacterInfo", fileName = "CharacterInfo")]
        public class CharacterInfo : ScriptableObject
        {
            public int _physique;
            public int _agility;
            public int _mind;
        }
}