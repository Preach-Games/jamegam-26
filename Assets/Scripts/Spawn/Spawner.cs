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

        private void Awake()
        {
            _spawnTime = SpawnManager.Instance.SpawnRate;
        }

        private void Start()
        {
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
            hero.GetComponent<ACharacter>().SetStaticTarget(_target.position);
        }
    }
}
