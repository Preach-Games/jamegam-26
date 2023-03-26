using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonDraws.Character
{
    public abstract class AEffect : MonoBehaviour
    {
        public abstract void Apply(ACharacter caster, ACharacter target);
    }
}
