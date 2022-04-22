using System;
using System.Collections;
using System.Collections.Generic;
using BulletFury;
using Ktyl.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Confined.Stages.Json2
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private TextAsset Stage1Asset;
        [SerializeField] private TextAsset Stage2Asset;
        [SerializeField] private TextAsset Stage3Asset;

        private Confined.Stages.Json2.Root LevelRoot;

        // Handle when all spawned game objects in a group have finished entering the scene
        private Dictionary<int, int> SpawnedGameObjects;
        public UnityEvent<int> GroupReadyToMoveEvent;

        // Handle when all spawned game objects in a group have died
        private Dictionary<int, int> EnemiesAliveByGroup;
        // private bool WaitForDestroyComplete = false;
        [SerializeField] private SerialFloat ScoreSerialFloat;

        [Header("Explosions")]
        [SerializeField] private BulletExplodeOnHit ExplodeScript;

        // Enemy prefabs - I know this should be done with addressables, but game jam.
        [Header("Enemies")]
        [SerializeField] private GameObject DroneXwbPrefab;

        private void Start()
        {
            SpawnedGameObjects = new Dictionary<int, int>();
            GroupReadyToMoveEvent = new UnityEvent<int>();

            EnemiesAliveByGroup = new Dictionary<int, int>();

            Debug.Log($"Loading level 1...");
            LevelRoot = JsonUtility.FromJson<Confined.Stages.Json2.Root>(Stage1Asset.text);

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
                                    StartCoroutine(SpawnDroneXwb(spawns[j], action.group));
                                    break;
                                default:
                                    break;
                            }
                        }

                        // WaitForDestroy action is running before the spawning has actually finished
                        yield return new WaitUntil(() => EnemiesAliveByGroup?.ContainsKey(action.group) == true);

                        break;
                    case "waitForDestroy":
                        Debug.Log($"waitForDestroy for group [{action.group}]...");
                        yield return new WaitUntil(() => EnemiesAliveByGroup?.ContainsKey(action.group) == false);
                        Debug.Log($"waitForDestroy for group [{action.group}] complete");
                        break;
                    case "delay":
                        Debug.Log("Delay action started...");
                        yield return new WaitForSeconds(action.delayTime);
                        Debug.Log("Delay action complete");
                        break;
                    default:
                        break;
                }

                yield return null;
            }
        }

        private IEnumerator SpawnDroneXwb(Spawn spawnInfo, int group)
        {
            // Delay spawn
            if (spawnInfo.entryDelay > 0.001f)
            {
                yield return new WaitForSeconds(spawnInfo.entryDelay);
            }

            // Setup
            var startPosition = new Vector3(spawnInfo.startingPosition.x, spawnInfo.startingPosition.y, 0);
            var drone = GameObject.Instantiate(DroneXwbPrefab, startPosition, Quaternion.identity);

            // Entry path
            var entryPath = drone.transform.Find("Entry Path");

            var p0 = entryPath.transform.Find("p0");
            p0.transform.localPosition = new Vector3(spawnInfo.entryPath.p0x, spawnInfo.entryPath.p0y, 0);

            var p1 = entryPath.transform.Find("p1");
            p1.transform.localPosition = new Vector3(spawnInfo.entryPath.p1x, spawnInfo.entryPath.p1y, 0);

            var p2 = entryPath.transform.Find("p2");
            p2.transform.localPosition = new Vector3(spawnInfo.entryPath.p2x, spawnInfo.entryPath.p2y, 0);

            var p3 = entryPath.transform.Find("p3");
            p3.transform.localPosition = new Vector3(spawnInfo.entryPath.p3x, spawnInfo.entryPath.p3y, 0);

            // Patrol path
            var pathController = drone.transform.Find("Path Controller");

            for (int i = 0; i < pathController.childCount; i++)
            {
                var point = pathController.GetChild(i);
                var patrolPoint = spawnInfo.patrolPoints[i];
                point.transform.localPosition = new Vector3(patrolPoint.x, patrolPoint.y, 0);
            }

            var component = drone.GetComponent<DroneXwb>();

            // Set the group
            if (group != 0)
            {
                component.SetGroup(group);
            }

            // Set the entry hover time
            if (spawnInfo.entryHoverTime != 0.0f)
            {
                component.SetEntryHoverTime(spawnInfo.entryHoverTime);
            }

            // Set the hover time
            if (spawnInfo.hoverTime != 0.0f)
            {
                component.SetHoverTime(spawnInfo.hoverTime);
            }

            // Subscribe to entry complete event
            component.EntryCompleteEvent.AddListener(HandleGroupEntryComplete);

            // Add a counter to the appropriate group
            if (SpawnedGameObjects?.ContainsKey(group) == true)
            {
                SpawnedGameObjects[group] += 1;
            }
            else
            {
                SpawnedGameObjects.Add(group, 1);
            }

            // Subscribe to group ready to move event
            GroupReadyToMoveEvent.AddListener(g => component.HandleGroupEntryComplete(g));

            // Explosion on hit - Bullet Manager OnBulletDied
            drone.GetComponent<BulletManager>().OnBulletDiedEvent += ExplodeScript.OnBulletDie;

            // Subscribe to death event
            component.DeathEvent.AddListener(HandleGroupDeathComplete);
            
            if (EnemiesAliveByGroup?.ContainsKey(group) == false)
            {
                EnemiesAliveByGroup.Add(group, 1);
            }
            else
            {
                EnemiesAliveByGroup[group] += 1;
            }

            yield return null;
        }

        private void HandleGroupEntryComplete(int group)
        {
            if (SpawnedGameObjects?[group] == null)
            {
                Debug.LogException(new Exception($"No spawned group with group number {group}."));
            }

            SpawnedGameObjects[group] -= 1;

            if (SpawnedGameObjects[group] == 0)
            {
                GroupReadyToMoveEvent?.Invoke(group);
            }
        }

        private void HandleGroupDeathComplete(int group, int pointValue)
        {
            // handle the case where an enemy in the group has spawned and is destroyed
            //  before the group entry has completed. removes that deadlock
            // if (SpawnedGameObjects?[group] == null)
            // {
            //     Debug.LogException(new Exception($"No spawned group with group number {group}."));
            // }

            // for the wait for destroy action
            if (EnemiesAliveByGroup?.ContainsKey(group) == true)
            {
                // add to the score
                ScoreSerialFloat.Value += pointValue;

                if (EnemiesAliveByGroup?[group] != 0)
                {
                    EnemiesAliveByGroup[group] -= 1;
                }

                if (EnemiesAliveByGroup?[group] == 0)
                {
                    EnemiesAliveByGroup.Remove(group);

                    // Enemies could have been destroyed before entry completed
                    SpawnedGameObjects[group] = 0;
                }
            }
        }
    }
}
