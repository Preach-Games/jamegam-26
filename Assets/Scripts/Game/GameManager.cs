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

        private float _dayTimer;

        public bool IsPaused { set; get; }

        private string[] _advices = new[]
        {
            "Only good people don't do their taxes so remember to do yours!",
            "Bring your monsters once a year at the veterinary for their annual checkup",
            "1403's adventurer reform specify that mimic can only be up to 20% of your chests",
            "Don't forget to speak to your monsters about their various syndicate options"
        };

        private void Awake()
        {
            Instance = this;
            _dayTimer = _info.DayDuration;
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
                }
            }
        }

        public void NextDay()
        {
            IsPaused = false;
        }
    }
}
