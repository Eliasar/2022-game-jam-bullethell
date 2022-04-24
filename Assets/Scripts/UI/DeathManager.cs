using Ktyl.Util;
using UnityEngine;

namespace Confined
{
    public class DeathManager : MonoBehaviour
    {
        [SerializeField] private GameObject DeathMenu;

        // Player Health properties
        [SerializeField] SerialFloat PlayerHealth;

        public void Pause()
        {
            DeathMenu.SetActive(true);
            Time.timeScale = 0.01f;
        }

        public void Retry()
        {
            DeathMenu.SetActive(false);
            Time.timeScale = 1f;
            PlayerHealth.Value = 100f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        public void Quit(int sceneId)
        {
            Time.timeScale = 1f;
            DeathMenu.SetActive(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
        }

        private void Update()
        {
            if (PlayerHealth <= 0.0f)
            {
                Pause();
            }
        }
    }
}