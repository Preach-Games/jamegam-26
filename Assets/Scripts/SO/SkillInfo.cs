using DungeonDraws.Character;
using DungeonDraws.Character.Skill.Effect;
using System;
using System.Collections.Generic;
using UnityEngine;
using static DungeonDraws.SO.SkillInfo;

namespace DungeonDraws.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/SkillInfo", fileName = "SkillInfo")]
    public class SkillInfo : ScriptableObject
    {
        [Flags]
        public enum TargetType
        {
            Self = 1,
            Enemy = 2,
            Ally = 4
        }

        public TargetType Type;
        public int MPCost;
        public int HPCost;
        public int Range;
        public EffectType[] Effects;


        public List<AEffect> _effects;

        private bool CanUse(ACharacter a, ACharacter b)
        {
            if (a == b)
            {
                return (Type & TargetType.Self) != 0;
            }
            if (a.Faction == b.Faction)
            {
                return (Type & TargetType.Ally) != 0;
            }
            return (Type & TargetType.Enemy) != 0;
        }

        public void Use(ACharacter caster, List<ACharacter> targets)
        {
            foreach (AEffect effect in _effects)
            {
                foreach (ACharacter target in targets)
                {
                    if (CanUse(caster, target))
                    {
                        effect.Apply(caster, target);
                    }
                }
            }
        }
    }
}