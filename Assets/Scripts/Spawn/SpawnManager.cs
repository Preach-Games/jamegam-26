using DungeonDraws.Character;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonDraws.Spawn
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance { private set; get; }

        private Transform _characterContainer;

        private readonly Dictionary<Faction, List<ACharacter>> _objects = new();

        [SerializeField]
        private GameObject _character;

        private void Awake()
        {
            Instance = this;
            _characterContainer = new GameObject("Characters").transform;
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
    }
}
