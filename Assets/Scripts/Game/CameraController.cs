using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonDraws.Game
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float _speed;

        private Vector2 _mov;

        private void Update()
        {
            transform.Translate(_mov * Time.deltaTime * _speed);
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            _mov = value.ReadValue<Vector2>().normalized;
        }
    }
}
