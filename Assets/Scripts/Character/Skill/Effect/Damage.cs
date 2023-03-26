using UnityEngine;

namespace DungeonDraws.Character
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

        Modifier _modifier = 0;

        public override void Apply(ACharacter caster, ACharacter target)
        {
            int dmgRoll = Random.Range(_diceNb, _diceNb * _diceFaces + 1);

            dmgRoll += _damageBonus;
            if (_modifier == Modifier.FORCE)
            {
                dmgRoll += caster._physique;
            }
            else if (_modifier == Modifier.AGILITY)
            {
                dmgRoll += caster._agility;
            }
            else if (_modifier == Modifier.MIND)
            {
                dmgRoll += caster._mind;
            }
            target.Hurt(dmgRoll);
        }
    }
}
