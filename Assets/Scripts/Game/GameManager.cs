using DungeonDraws.Card;
using DungeonDraws.SO;
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

        private int _gold;

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
            _gold = _info.BaseGold;
        }

        private void Update()
        {
            if (!IsPaused)
            {
                _dayTimer -= Time.deltaTime;
                if (_dayTimer < 0f)
                {
                    _dayTimer = _info.DayDuration;
                    CardsManager.Instance.ResetDay();
                    IsPaused = true;
                    _nextDayPanel.SetActive(true);
                    _nextDayAdvice.text = $"Tip: {_advices[Random.Range(0, _advices.Length)]}";

                    _gold += _info.DailyIncome;
                    _incomeText.text = $"Net Income: {_info.DailyIncome} Gold Coin\nTotal Gold: {_gold}";
                }
            }
        }

        public void NextDay()
        {
            IsPaused = false;
        }
    }
}
