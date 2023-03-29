using DungeonDraws.Character;
using DungeonDraws.Game;
using DungeonDraws.SO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonDraws.Spawn
{
    public enum SpawnMethod
    {
        Random,
        CentreOfRoom,
        FurthestWallFromPoint
    }
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField]
        private GameInfo _info;

        public static SpawnManager Instance { private set; get; }

        private Transform _characterContainer;

        private readonly Dictionary<Faction, List<ACharacter>> _objects = new();

        [SerializeField]
        private GameObject _character;

        [SerializeField]
        private Transform[] _spawns;

        public float SpawnRate { private set; get; }

        private void Awake()
        {
            Instance = this;
            _characterContainer = new GameObject("Characters").transform;
            SpawnRate = _info.TimeBetweenSpawn;

            GameStatusHandler.Instance.OnDayReset += (_sender, _e) =>
            {
                // Remove all heroes
                if (_objects.ContainsKey(Faction.OVERLORD))
                {
                    foreach (var elem in _objects[Faction.OVERLORD])
                    {
                        elem.UnsetTarget();
                    }
                }
                if (_objects.ContainsKey(Faction.HERO))
                {
                    foreach (var elem in _objects[Faction.HERO])
                    {
                        Destroy(elem.gameObject);
                    }
                    _objects[Faction.HERO].Clear();
                }

                // Reset spawn rate
                SpawnRate = _info.TimeBetweenSpawn;
            };
        }

        public void Spawn(Race race, SpawnMethod spawnMethod)
        {
            SpawnAtRandom(_info.Enemies.FirstOrDefault(x => x.Race == race));
        }

        private void SpawnAtRandom(SO.CharacterInfo info)
        {
            Vector2 pos = _spawns[Random.Range(0, _spawns.Length)].position;
            Spawn(info, new Vector3(pos.x, 1f, pos.y));
        }

        public GameObject Spawn(SO.CharacterInfo info, Vector3 pos)
        {
            var go = Instantiate(_character, _characterContainer);
            go.transform.position = pos;
            var sub = Instantiate(info.Prefab, go.transform);
            sub.transform.rotation = info.Prefab.transform.rotation;
            var charac = go.GetComponent<ACharacter>();
            charac.Info = info;
            if (!_objects.ContainsKey(charac.FactionOverride))
            {
                _objects.Add(charac.FactionOverride, new());
            }
            _objects[charac.FactionOverride].Add(charac);
            return go;
        }

        public void Die(ACharacter c)
        {
            _objects[c.FactionOverride].Remove(c);
            Destroy(c.gameObject);
        }

        public void TakePercentDamage(int value, Faction faction)
        {
            if (!_objects.ContainsKey(faction))
            {
                return;
            }
            foreach (var elem in _objects[faction])
            {
                elem.TakePercentDamage(value);
            }
        }

        public void TakePercentAttack(int value, Faction faction)
        {
            if (!_objects.ContainsKey(faction))
            {
                return;
            }
            foreach (var elem in _objects[faction])
            {
                elem.AttackModifier += value;
            }
        }

        public void ChangeSpawnRate(int percent)
        {
            SpawnRate += (_info.TimeBetweenSpawn * percent / 100f);
        }
    }
}
