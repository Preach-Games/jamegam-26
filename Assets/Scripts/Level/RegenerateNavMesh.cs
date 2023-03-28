using DungeonDraws.Scripts.Utils.Attributes;
using Unity.AI.Navigation;
using UnityEngine;

namespace Assets.Scripts.Level
{
    public class RegenerateNavMesh : MonoBehaviour
    {
        [Button]
        public void Regenerate()
        {
            GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}
