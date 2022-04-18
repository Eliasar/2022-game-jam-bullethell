using System.Collections;
using System.IO;
using UnityEngine;

namespace Confined.Stages.Json
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private string StagePath;

        private Confined.Json2.Root root;

        private void Start()
        {
            using (StreamReader stream = new StreamReader(StagePath))
            {
                string json = stream.ReadToEnd();
                root = JsonUtility.FromJson<Confined.Json2.Root>(json);
            }

            // Debug.Log($"root: {root}");
            Debug.Log($"Loaded level [{root.levelName}]");

            // Play the actions, coroutine, duh
            StartCoroutine(PlayActions(root.actions));
        }

        private IEnumerator PlayActions(Confined.Json2.Action[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                var action = actions[i];

                switch (action.type)
                {
                    case "spawn":
                        // Find the prefab

                        // Instantiate on the timer
                        break;
                    case "waitForDestroy":
                        break;
                    case "delay":
                        break;
                    default:
                        break;
                }

                yield return null;
            }

        }
    }
}
