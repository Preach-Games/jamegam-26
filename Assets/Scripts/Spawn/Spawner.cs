using DungeonDraws.Character;
using DungeonDraws.Game;
using System.Collections;
using UnityEngine;

namespace DungeonDraws.Spawn
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private SO.CharacterInfo _toSpawn;

        [SerializeField]
        private Transform _target;

        private float _spawnTime;

        private void Start()
        {
            _spawnTime = SpawnManager.Instance.SpawnRate;
            GameManager.Instance.OnDayReset += (_sender, _e) =>
            {
                _spawnTime = SpawnManager.Instance.SpawnRate;
            };
        }

        private void Update()
        {
            if (!GameManager.Instance.IsPaused)
            {
                _spawnTime -= Time.deltaTime;
                if (_spawnTime <= 0f)
                {
                    Spawn();
                    _spawnTime = SpawnManager.Instance.SpawnRate;
                }
            }
        }

        private void Spawn()
        {
            var hero = SpawnManager.Instance.Spawn(_toSpawn, transform.position);
            hero.GetComponent<ACharacter>().SetGoal(_target);
        }
    }
}
