using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonDraws
{
    public class MainMenu : MonoBehaviour
    {
        public void LoadGame()
        {
            SceneManager.LoadScene("Cards");
        }
    }
}
