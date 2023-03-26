using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonDraws.Character
{
    public class Skill : MonoBehaviour
    {
        // 
        // 0 = self only
        // 1 = Enemy only
        // 2 = Self or Ally only
        // 3 = Self, Ally or Enemy
        //
        public int _targetType;

        public int range;

        public List<AEffect> _effects;

        public void Use(ACharacter caster, List<ACharacter> targets)
        {
            ;
        }
    }
}