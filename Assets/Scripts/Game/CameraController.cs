using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonDraws.Game
{
    public class CameraController : MonoBehaviour
    {
        public void OnMove(InputAction.CallbackContext value)
        {
            _ = value.ReadValue<Vector2>().normalized;
        }
    }
}
