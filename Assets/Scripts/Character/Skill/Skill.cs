using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonDraws.Character
{
    public class Skill : MonoBehaviour
    {
        [Flags]
        private enum TargetType
        {
            Self = 1,
            Enemy = 2,
            Ally = 4
        }
        public int _targetType;

        public int range;

        public List<AEffect> _effects;

        public void Use(ACharacter caster, List<ACharacter> targets)
        {
            ;
        }
    }
}