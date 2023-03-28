using DungeonDraws.Game;
using DungeonDraws.Spawn;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonDraws.Character
{
    public abstract class ACharacter : MonoBehaviour
    {
        public SO.CharacterInfo Info { set; private get; }

        private NavMeshAgent _agent;

        private ACharacter _target;
        private Transform _goal;
        private Vector3 _spawn;

        private Rigidbody2D _rb;

        private float _attackTimerRef = 2f;
        private float _attackTimer;

        private int _gold;

        public int AttackModifier { get; set; }

        public void SetGoal(Transform t)
        {
            _agent.SetDestination(t.position);
            _goal = t;
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
                var p = other.GetComponent<ACharacter>() ?? other.transform.parent.GetComponent<ACharacter>();
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
            if (GameManager.Instance.IsPaused)
            {
                _agent.isStopped = true;
                _rb.velocity = Vector2.zero;
                return;
            }
            _attackTimer -= Time.deltaTime;
            try
            {
                _agent.isStopped = _target != null && Vector3.Distance(transform.position, _target.transform.position) < 2.5f;
            }
            catch (MissingReferenceException)
            {
                _agent.isStopped = true;
                _target = null;
            }
            if (_agent.isStopped)
            {
                _rb.velocity = Vector2.zero;
            }
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
            else
            {
                if (_goal == null)
                {
                    _agent.SetDestination(_spawn);
                }
                else
                {
                    if (Vector3.Distance(transform.position, _goal.position) < 2f)
                    {
                        _goal = null;
                        _gold = 10;
                        GameManager.Instance.AddExpenses(_gold, 0);
                    }
                    else
                    {
                        _agent.SetDestination(_goal.position);
                    }
                }
            }
        }

        public void Attack(ACharacter target)
        {
            int rollA = Random.Range(1, 21);
            int rollD = Random.Range(1, 21);

            //if (rollA + Agility > rollD + target.Agility)
            {
                target.Hurt(Mathf.FloorToInt(Physique * (1 + AttackModifier / 100f)));
            }
        }

        public void TakePercentDamage(int percent)
        {
            Hurt(Mathf.FloorToInt(_hpMax * percent / 100f));
        }

        private void Die()
        {
            GameManager.Instance.AddExpenses(-_gold, 0);
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
            _rb = GetComponent<Rigidbody2D>();
            _attackTimer = _attackTimerRef;
            _spawn = transform.position;
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
            GameManager.Instance.DisplayDamage(transform.position, dmg);
            _hp -= dmg;
            if (_hp > _hpMax)
            {
                _hp = _hpMax;
            }
            CheckStatus();
        }
        #endregion
    }
}