using UnityEngine;

namespace DungeonDraws.Character
{
    public abstract class ACharacter : MonoBehaviour
    {

        [SerializeField]
        public SO.CharacterInfo _info;

        public int _physique;
        public int _agility;
        public int _mind;
        public int _faction;

        private int _hp;
        private int _hpMax;
        private int _mp;
        private int _mpMax;
        private int _init;

        private int _status = 1;

        private List<Skill> _skillList;

        private void Awake()
        {
            if (_info == null)
            {
                throw new System.ArgumentNullException();
            }
            _physique = _info._physique;
            _agility = _info._agility;
            _mind = _info._mind;
            _faction = Side();
            _hp = 10 + _physique * 2;
            _hpMax = _hp;
            _mp = 5 + _mind * 2;
            _mpMax = _mp;
            _init = _agility * 2;
        }

        public int Side()
        {
            return 0;
        }

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
            int rollA = Random.Range(1, 21);
            int rollD = Random.Range(1, 21);

            if (rollA + _agility > rollD + target._agility)
            {
                target.Hurt(_physique);
            }
        }
    }
}