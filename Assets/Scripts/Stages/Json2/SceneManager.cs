using System.Collections;
using UnityEngine;

namespace Confined.Stages.Json2
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private TextAsset Stage1Asset;
        [SerializeField] private TextAsset Stage2Asset;
        [SerializeField] private TextAsset Stage3Asset;

        private Confined.Stages.Json2.Root LevelRoot;

        // Enemy prefabs - I know this should be done with addressables, but game jam.
        [Header("Enemies")]
        [SerializeField] private GameObject DroneXwbPrefab;

        private void Start()
        {
            Debug.Log($"Loading level 1...");
            LevelRoot = JsonUtility.FromJson<Confined.Stages.Json2.Root>(Stage1Asset.text);

            // Debug.Log($"root: {root}");
            Debug.Log($"Loaded level [{LevelRoot.levelName}]");

            // Play the actions, coroutine, duh
            StartCoroutine(PlayActions(LevelRoot.actions));
        }

        private IEnumerator PlayActions(Confined.Stages.Json2.Action[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                var action = actions[i];

                switch (action.type)
                {
                    case "spawn":
                        // Get all spawns
                        var spawns = action.spawns;
                        
                        // For every spawn, get the prefab and instantiate based on timer
                        for (int j = 0; j < spawns.Length; j++)
                        {
                            switch (spawns[j].prefab)
                            {
                                case "Drone-XWB":
                                    // var drone = GameObject.Instantiate(DroneXwb);
                                    StartCoroutine(SpawnDroneXwb(spawns[j]));
                                    break;
                                default:
                                    break;
                            }

                            yield return null;
                        }

                        // Instantiate on the timer
                        // Instantiate for now, pool objects later

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

        private IEnumerator SpawnDroneXwb(Spawn spawnInfo)
        {
            // Setup
            var startPosition = new Vector3(spawnInfo.startingPosition.x, spawnInfo.startingPosition.y, 0);
            var drone = GameObject.Instantiate(DroneXwbPrefab, startPosition, Quaternion.identity);

            // Entry path
            var entryPath = drone.transform.Find("Entry Path");

            var p0 = entryPath.transform.Find("p0");
            p0.transform.position = new Vector3(spawnInfo.entryPath.p0x, spawnInfo.entryPath.p0y, 0);

            var p1 = entryPath.transform.Find("p1");
            p1.transform.position = new Vector3(spawnInfo.entryPath.p1x, spawnInfo.entryPath.p1y, 0);

            var p2 = entryPath.transform.Find("p2");
            p2.transform.position = new Vector3(spawnInfo.entryPath.p2x, spawnInfo.entryPath.p2y, 0);

            var p3 = entryPath.transform.Find("p3");
            p3.transform.position = new Vector3(spawnInfo.entryPath.p3x, spawnInfo.entryPath.p3y, 0);

            // Patrol path
            var pathController = drone.transform.Find("Path Controller");

            for (int i = 0; i < pathController.childCount; i++)
            {
                var point = pathController.GetChild(i);
                var patrolPoint = spawnInfo.patrolPoints[i];
                point.transform.position = new Vector3(patrolPoint.x, patrolPoint.y, 0);
            }

            // Delay as indicated
            yield return new WaitForSeconds(spawnInfo.entryDelay);

            yield return null;
        }
    }
}
