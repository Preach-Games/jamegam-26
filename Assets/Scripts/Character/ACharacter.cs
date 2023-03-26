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

        public void SetStaticTarget(Vector3 pos)
        {
            _agent.SetDestination(pos);
            _target = null;
        }

        public void SetDynamicTarget(Transform t)
        {
            _agent.SetDestination(_target.position);
            _target = t;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var p = other.GetComponent<ACharacter>();
                //if (Faction != p.Faction) TODO: I don't really care rn
                {
                    SetDynamicTarget(other.transform);
                }
            }
        }

        private void Update()
        {
            _agent.enabled = _target == null || Vector2.Distance(transform.position, _target.position) < 2f;
            if (_target != null)
            {
                if (_agent.enabled)
                {
                    _agent.SetDestination(_target.position);
                }
                else
                {
                    Debug.Log("We can attack");
                }
            }
        }

        public void Attack(ACharacter target)
        {
            int rollA = Random.Range(1, 21);
            int rollD = Random.Range(1, 21);

            if (rollA + Agility > rollD + target.Agility)
            {
                target.Hurt(Physique);
            }
        }

        public static bool operator ==(ACharacter a, ACharacter b)
        {
            if (a is null) return b is null;
            if (b is null) return false;
            return a.GetInstanceID() == b.GetInstanceID();
        }

        public static bool operator !=(ACharacter a, ACharacter b)
            => !(a == b);

        public override bool Equals(object obj)
        {
            return obj is ACharacter character && this == character;
        }

        public override int GetHashCode()
        {
            return GetInstanceID().GetHashCode();
        }

        #region StatsStuffs

        public int Physique { private set; get; }
        public int Agility { private set; get; }
        public int Mind { private set; get; }

        private int _hp;
        private int _hpMax;
        private int _mp;
        private int _mpMax;
        private int _init;
        private int _status;

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
        #endregion
    }
}