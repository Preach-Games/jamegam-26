using DungeonDraws.SO;
using UnityEngine;

namespace DungeonDraws.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameInfo _info;

        private void Awake()
        {
            Instance = this;
        }
    }
}
