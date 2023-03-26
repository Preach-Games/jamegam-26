using DungeonDraws.Character;
using System.Collections;
using UnityEngine;

namespace DungeonDraws.Spawn
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _toSpawn;

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
            var hero = Instantiate(_toSpawn, transform.position, Quaternion.identity);
            hero.GetComponent<ACharacter>().Target = _target;
            yield return new WaitForSeconds(_spawnTimeRef);
            yield return Spawn();
        }
    }
}
