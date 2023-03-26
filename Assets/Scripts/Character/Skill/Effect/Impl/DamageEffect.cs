using UnityEngine;

namespace DungeonDraws.Character.Skill.Effect.Impl
{
    public class DamageEffect : AEffect
    {
        public enum Modifier
        {
            NONE,
            FORCE,
            AGILITY,
            MIND
        }

        int _diceNb = 0;
        int _diceFaces = 0;
        int _damageBonus = 0;

        Modifier _modifier = Modifier.FORCE;

        public override void Apply(ACharacter caster, ACharacter target)
        {
            int dmgRoll = Random.Range(_diceNb, _diceNb * _diceFaces + 1);

            dmgRoll += _damageBonus;
            if (_modifier == Modifier.FORCE)
            {
                dmgRoll += caster.Physique;
            }
            else if (_modifier == Modifier.AGILITY)
            {
                dmgRoll += caster.Agility;
            }
            else if (_modifier == Modifier.MIND)
            {
                dmgRoll += caster.Mind;
            }
            target.Hurt(dmgRoll);
        }
    }
}
