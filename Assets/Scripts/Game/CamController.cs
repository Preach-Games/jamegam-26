using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonDraws.Game
{
    public class CamController : MonoBehaviour
    {
        [SerializeField]
        private float _speed;

        private Vector2 _mov;

        private void Update()
        {
            transform.Translate(new Vector3(_mov.x, 0f, _mov.y) * Time.deltaTime * _speed, Space.World);
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            _mov = value.ReadValue<Vector2>().normalized;
        }
    }
}
