using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonDraws.Scripts.Utils
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}