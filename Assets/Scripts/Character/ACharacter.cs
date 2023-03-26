using DungeonDraws.Spawn;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace DungeonDraws.Character
{
    public abstract class ACharacter : MonoBehaviour
    {
        public SO.CharacterInfo Info { set; private get; }

        private NavMeshAgent _agent;

        private ACharacter _target;

        private float _attackTimerRef = 2f;
        private float _attackTimer;

        public void SetStaticTarget(Vector3 pos)
        {
            _agent.SetDestination(pos);
            _target = null;
        }

        public void SetDynamicTarget(ACharacter t)
        {
            _agent.SetDestination(t.transform.position);
            _target = t;
        }

        public void UnsetTarget()
        {
            _target = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var p = other.GetComponent<ACharacter>();
                if (FactionOverride != p.FactionOverride)
                {
                    SetDynamicTarget(p);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var p = other.GetComponent<ACharacter>();
                if (p == _target)
                {
                    _target = null;
                }
            }
        }

        private void Update()
        {
            _attackTimer -= Time.deltaTime;
            _agent.isStopped = _target != null && Vector3.Distance(transform.position, _target.transform.position) < 2f;
            if (_target != null)
            {
                if (_agent.isStopped)
                {
                    if (_attackTimer <= 0f)
                    {
                        _attackTimer = _attackTimerRef;
                        Attack(_target);
                    }
                }
                else
                {
                    _agent.SetDestination(_target.transform.position);
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

        public void TakePercentDamage(int percent)
        {
            Hurt(Mathf.FloorToInt(_hpMax * percent / 100f));
        }

        private void Die()
        {
            SpawnManager.Instance.Die(this);
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

        protected void AwakeInternal()
        {
            _agent = GetComponent<NavMeshAgent>();
            _attackTimer = _attackTimerRef;
        }

        protected void StartInternal()
        {
            Physique = Info.Physique;
            Agility = Info.Agility;
            Mind = Info.Mind;
            _hp = 10 + Physique * 2;
            _hpMax = _hp;
            _mp = 5 + Mind * 2;
            _mpMax = _mp;
            _init = Agility * 2;
        }

        public Faction FactionOverride => Info.Faction; // TODO: Replace that with your things later on
        public abstract int Faction { get; }

        public void CheckStatus()
        {
            if (_hp < 0)
            {
                Die();
            }

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