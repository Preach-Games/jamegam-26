using UnityEngine;
using UnityEngine.AI;

namespace DungeonDraws.Character
{
    public class MaterialController : MonoBehaviour
    {
        [SerializeField]
        private Material _ondulationMat;
        private NavMeshAgent _agent;
        private const float _maxMovementSpeed = -28f;
        private const float _maxAgentSpeedApprox = 3.5f;

        void Start()
        {
            _agent = transform.parent.GetComponent<NavMeshAgent>();
            _ondulationMat.SetFloat("_MovementSpeed", 0);
        }

        void Update()
        {
            float velocity = _agent.velocity.magnitude;
            float t = Mathf.InverseLerp(0, _maxAgentSpeedApprox, _agent.velocity.magnitude);
            float speed = velocity > 0.5f ? Mathf.Lerp(0, _maxMovementSpeed, t) : 0;

            _ondulationMat.SetFloat("_MovementSpeed", speed);
        }
    }
}