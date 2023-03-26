using UnityEngine;

namespace DungeonDraws.Character.Skill.Effect
{
    public abstract class AEffect : MonoBehaviour
    {
        public abstract void Apply(ACharacter caster, ACharacter target);
    }
}
