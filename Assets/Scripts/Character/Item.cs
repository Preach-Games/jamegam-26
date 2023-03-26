using System.Collections.Generic;
using UnityEngine;

namespace DungeonDraws.Character
{
    public class Item : MonoBehaviour
    {
        //public string name;
        public string description;

        public List<Skill> skills;

        public void Use(ACharacter caster, ACharacter target, Skill skillToUse)
        {
            ;
        }
    }
}