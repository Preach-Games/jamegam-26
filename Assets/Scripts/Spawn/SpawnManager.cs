using UnityEngine;

namespace DungeonDraws.Spawn
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance { private set; get; }

        private Transform _characterContainer;

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
            var character = Instantiate(info.Prefab, go.transform);
            return go;
        }
    }
}
