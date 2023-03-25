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

        private float _dayTimer;

        private void Awake()
        {
            Instance = this;
            _dayTimer = _info.DayDuration;
        }

        private void Update()
        {
            if (!CardsManager.Instance.IsPaused)
            {
                _dayTimer -= Time.deltaTime;
                if (_dayTimer < 0f)
                {
                    _dayTimer = _info.DayDuration;
                    CardsManager.Instance.ResetDay();
                }
            }
        }
    }
}
