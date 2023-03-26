using DungeonDraws.Character;
using DungeonDraws.Game;
using DungeonDraws.SO;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonDraws.Spawn
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField]
        private GameInfo _info;

        public static SpawnManager Instance { private set; get; }

        private Transform _characterContainer;

        private readonly Dictionary<Faction, List<ACharacter>> _objects = new();

        [SerializeField]
        private GameObject _character;

        public float SpawnRate { private set; get; }

        private void Awake()
        {
            Instance = this;
            _characterContainer = new GameObject("Characters").transform;
            SpawnRate = _info.TimeBetweenSpawn;

            GameManager.Instance.OnDayReset += (_sender, _e) =>
            {
                // Remove all heroes
                foreach (var elem in _objects[Faction.HERO])
                {
                    Destroy(elem.gameObject);
                }
                _objects[Faction.HERO].Clear();

                // Reset spawn rate
                SpawnRate = _info.TimeBetweenSpawn;
            };
        }

        public GameObject Spawn(SO.CharacterInfo info, Vector3 pos)
        {
            var go = Instantiate(_character, _characterContainer);
            go.transform.position = pos;
            Instantiate(info.Prefab, go.transform);
            var charac = go.GetComponent<ACharacter>();
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

        public void ChangeSpawnRate(int percent)
        {
            SpawnRate += (_info.TimeBetweenSpawn * percent / 100f);
        }
    }
}
