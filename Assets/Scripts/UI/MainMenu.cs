using Ktyl.Util;
using UnityEngine;

namespace Confined.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private SerialFloat PlayerHealth;

        public void LoadScene(int sceneId)
        {
            Time.timeScale = 1f;
            PlayerHealth.Value = 100f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}