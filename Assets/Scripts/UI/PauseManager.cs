using UnityEngine;

namespace Confined
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private GameObject PauseMenu;

        public void Pause()
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        public void Resume()
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }

        public void LoadMainScreenScene(int sceneId)
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }
    }
}