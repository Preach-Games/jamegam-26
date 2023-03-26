using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace DungeonDraws.Character
{
    public abstract class ACharacter : MonoBehaviour
    {
        [SerializeField]
        private SO.CharacterInfo _info;

        private NavMeshAgent _agent;

        private Transform _target;
        public Transform Target
        {
            set
            {
                _target = value;
                _agent.SetDestination(_target.position);
            }
            get => _target;
        }

        public int Physique { private set; get; }
        public int Agility { private set; get; }
        public int Mind { private set; get; }

        private int _hp;
        private int _hpMax;
        private int _mp;
        private int _mpMax;
        private int _init;

        private int _status;

        private List<Skill> _skillList;

        protected void Init()
        {
            _agent = GetComponent<NavMeshAgent>();

            Assert.IsNotNull(_info);
            Physique = _info.Physique;
            Agility = _info.Agility;
            Mind = _info.Mind;
            _hp = 10 + Physique * 2;
            _hpMax = _hp;
            _mp = 5 + Mind * 2;
            _mpMax = _mp;
            _init = Agility * 2;
        }

        public abstract int Faction { get; }

        public void CheckStatus()
        {
            if (_hp > 0)
            {
                _status = 1;
            }
            else if (_hp > (_hpMax / 2) * -1)
            {
                _status = 0;
            }
            else
            {
                _status = -1;
            }
        }

        public void Hurt(int dmg)
        {
            _hp -= dmg;
            CheckStatus();
        }

        public void Attack(ACharacter target) 
        {
            int rollA = UnityEngine.Random.Range(1, 21);
            int rollD = UnityEngine.Random.Range(1, 21);

            if (rollA + Agility > rollD + target.Agility)
            {
                target.Hurt(Physique);
            }
        }
    }
}