using DungeonDraws.Effect;
using DungeonDraws.SO;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace DungeonDraws.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private GameObject _nextDayPanel;

        [SerializeField]
        private TMP_Text _nextDayAdvice;

        [SerializeField]
        private TMP_Text _incomeText;

        [SerializeField]
        private GameObject _damageDisplayPrefab;

        [SerializeField]
        private TMP_Text _goldLabel;

        private float _dayTimer;

        public bool IsPaused { set; get; }

        private int _gold;
        public int Gold
        {
            set
            {
                _gold = value;
                DisplayGold();
            }
            get => _gold;
        }

        int[] _upcomingExpenses;

        private string[] _advices = new[]
        {
            "Only good people don't do their taxes so remember to do yours!",
            "Bring your monsters at the veterinary once a year for their annual checkup",
            "1403's adventurer reform specify that mimic can only be up to 20% of your chests",
            "Don't forget to speak to your monsters about their various syndicate options"
        };

        private void Awake()
        {
            Instance = this;
            _dayTimer = _info.DayDuration;
            Gold = _info.BaseGold;
            _upcomingExpenses = Enumerable.Repeat(0, 10).ToArray();
        }

        public event EventHandler OnDayReset;

        private void Update()
        {
            if (!IsPaused)
            {
                _dayTimer -= Time.deltaTime;
                if (_dayTimer < 0f)
                {
                    _dayTimer = _info.DayDuration;
                    OnDayReset.Invoke(this, new());
                    IsPaused = true;
                    _nextDayPanel.SetActive(true);
                    _nextDayAdvice.text = $"Tip: {_advices[UnityEngine.Random.Range(0, _advices.Length)]}";

                    Gold += _info.DailyIncome;
                    Gold -= _upcomingExpenses[0];
                    _incomeText.text = $"Net Income: {_info.DailyIncome - _upcomingExpenses[0]} Gold Coin\nTotal Gold: {Gold}";
                    for (int i = 0; i < _upcomingExpenses.Length - 1; i++)
                    {
                        _upcomingExpenses[i] = _upcomingExpenses[i + 1];
                    }
                    _upcomingExpenses[^1] = 0;
                    DisplayGold();
                    if (Gold <= 0)
                    {
                        // Game Over
                    }
                }
            }
        }

        private void DisplayGold()
        {
            var futureExpenses = _upcomingExpenses == null ?  0 : _upcomingExpenses.Sum();
            _goldLabel.text = $"Gold: {Gold} ({(futureExpenses > 0 ? "-" : "+")}{Mathf.Abs(futureExpenses)})";
        }

        public void AddExpenses(int amount, int days)
        {
            _upcomingExpenses[days] += amount;
            DisplayGold();
        }

        public void AddExpensesPercent(int amount)
        {
            _upcomingExpenses[0] += Mathf.CeilToInt(_info.DailyIncome * amount / 100f);
            DisplayGold();
        }

        public void NextDay()
        {
            IsPaused = false;
        }

        public void DisplayDamage(Vector3 pos, int amount)
        {
            var go = Instantiate(_damageDisplayPrefab, pos + Vector3.up, Quaternion.identity);
            go.GetComponent<DamageDisplay>().Init(amount);
        }
    }
}
