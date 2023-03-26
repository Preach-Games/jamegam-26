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
        public TargetType _targetType;

        public int _mpCost;
        public int _hpCost;
        public int _range;

        public List<AEffect> _effects;

        public void Use(ACharacter caster, List<ACharacter> targets)
        {
            foreach (AEffect effect in _effects) {
                foreach (ACharacter target in targets) {
                    if (caster.Side() == target.Side() &&
                        _targetType & TargetType.Ally != 0 &&
                        caster.gameObject.GetInstanceID() != caster.gameObject.GetInstanceID()) {
                        effect.Apply(caster, target);
                    } else if (caster.Side() != target.Side() &&
                            _targetType & TargetType.Enemy != 0) {
                        effect.Apply(caster, target);
                    } else if (_targetType & TargetType.Self != 0 &&
                        caster.gameObject.GetInstanceID() == caster.gameObject.GetInstanceID()) {
                        effect.Apply(caster, target);
                    }
                }
            }
        }
    }
}