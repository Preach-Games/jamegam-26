using DungeonDraws.Character;
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

        [SerializeField]
        private float _spawnTimeRef;

        private void Awake()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            var hero = SpawnManager.Instance.Spawn(_toSpawn, transform.position);
            hero.GetComponent<ACharacter>().SetStaticTarget(_target.position);
            yield return new WaitForSeconds(_spawnTimeRef);
            yield return Spawn();
        }
    }
}
