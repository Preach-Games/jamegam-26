using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonDraws.Character
{
    public class DamageEffect : AEffect
    {
        int _diceNb = 0;
        int _diceFaces = 0;
        int _damageBonus = 0;

        // 
        // 0 = none
        // 1 = Physique
        // 2 = Agility
        // 3 = Mind
        //
        int _modifier = 0;

        public override void Apply(ACharacter caster, ACharacter target)
        {
            int dmgRoll = Random.Range(_diceNb, _diceNb * _diceFaces + 1);

            dmgRoll += _damageBonus;
            if (_modifier == 1)
            {
                dmgRoll += caster._physique;
            }
            else if (_modifier == 2)
            {
                dmgRoll += caster._agility;
            }
            else if (_modifier == 3)
            {
                dmgRoll += caster._mind;
            }
            target.Hurt(dmgRoll);
        }
    }
}
