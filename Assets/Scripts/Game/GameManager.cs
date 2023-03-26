using DungeonDraws.Card;
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

        private float _dayTimer;

        public bool IsPaused { set; get; }

        public int Gold { set; get; }

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
                    if (Gold <= 0)
                    {
                        // Game Over
                    }
                }
            }
        }

        public void AddExpenses(int amount, int days)
        {
            _upcomingExpenses[days] += amount;
        }

        public void AddExpensesPercent(int amount)
        {
            _upcomingExpenses[0] += Mathf.CeilToInt(_info.DailyIncome * amount / 100f);
        }

        public void NextDay()
        {
            IsPaused = false;
        }
    }
}
