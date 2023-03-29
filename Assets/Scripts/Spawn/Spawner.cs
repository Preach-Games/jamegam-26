using DungeonDraws.Character;
using DungeonDraws.Game;
using TMPro;
using UnityEngine;

namespace DungeonDraws.Spawn
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private SO.CharacterInfo _toSpawn;

        [SerializeField]
        private Transform _target;

        [SerializeField]
        private TMP_Text _nextSpawn;

        private float _spawnTime;

        private void Start()
        {
            _spawnTime = GetSpawnTime();
            _nextSpawn.text = $"Next Spawn: {Mathf.CeilToInt(_spawnTime)}";
            GameStatusHandler.Instance.OnDayReset += (_sender, _e) =>
            {
                _spawnTime = GetSpawnTime();
                _nextSpawn.text = $"Next Spawn: {Mathf.CeilToInt(_spawnTime)}";
            };
        }

        private float GetSpawnTime()
        {
            var minus = GameManager.Instance.RoundCount;
            var res = SpawnManager.Instance.SpawnRate - minus;
            if (res < 2) return 2f;
            return res;
        }

        private void Update()
        {
            if (!GameManager.Instance.IsPaused)
            {
                _spawnTime -= Time.deltaTime;
                _nextSpawn.text = $"Next Spawn: {Mathf.CeilToInt(_spawnTime)}";
                if (_spawnTime <= 0f)
                {
                    Spawn();
                    _spawnTime = GetSpawnTime();
                }
            }
        }

        private void Spawn()
        {
            var hero = SpawnManager.Instance.Spawn(_toSpawn, transform.position);
            hero.GetComponent<ACharacter>().SetGoal(_target);
            _nextSpawn.text = "Next Spawn: 0";
        }
    }
}
