using DungeonDraws.Card;
using DungeonDraws.SO;
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

        private float _dayTimer;

        public bool IsPaused { set; get; }

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
                }
            }
        }

        public void NextDay()
        {
            IsPaused = false;
        }
    }
}
