using BulletFury;
using BulletFury.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Confined
{
    public class DroneXwb : MonoBehaviour
    {
        [Header("Entering")]
        [SerializeField] private float EntrySpeed = 45.0f;
        [SerializeField] private float EntryHoverTime = 1.5f;
        [SerializeField] private BezierPath EntryPath;
        private float entryTParam = 0.0f;
        private bool IsEntryComplete = false;

        public UnityEvent<int> EntryCompleteEvent;

        [Header("Position")]
        [SerializeField] private float Speed = 15.0f;
        [SerializeField] private float HoverTime = 2.0f;
        [SerializeField] private PathController PathController;
        private Vector3 targetPosition;

        [Header("Bullets")]
        [SerializeField] private BulletManager bulletManager = null;
        [SerializeField] private int MaxBullets = 5;
        [SerializeField] private Transform PlayerTransform = null;
        private int currentBulletIndex;

        [Header("Death")]
        [SerializeField] private GameObject ExplosionPrefab = null;
        [SerializeField] private Mesh ExplosionMesh;
        [SerializeField] private Material ExplosionMaterial;
        [SerializeField] private int NumberOfExplosions = 30;
        [SerializeField] private int PointValue = 100;

        public UnityEvent<int, int> DeathEvent;
        private bool CanDestroy = false;

        // Meta
        private int Group = 0;

        // States
        public enum DroneState
        {
            Entering,
            Hovering,
            Shooting,
            Moving,
            Dying
        }
        public DroneState State;
        
        public void SetGroup(int group)
        {
            Group = group;
        }

        public void SetEntryHoverTime(float time)
        {
            EntryHoverTime = time;
        }

        private void Start()
        {
            currentBulletIndex = 0;
            entryTParam = 0.0f;
            
            PathController = GetComponentInChildren<PathController>();
            targetPosition = PathController.GetFirstPoint();
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            State = DroneState.Entering;
        }

        // Update is called once per frame
        void Update()
        {
            if (bulletManager == null || State == DroneState.Dying)
            {
                return;
            }

            // Look at the player
            transform.up = PlayerTransform.position - transform.position;

            switch (State)
            {
                case DroneState.Entering:
                    // The final target position will be p3 of the Bezier curve
                    if (entryTParam >= 1)
                    {
                        StartCoroutine(OnHover(EntryHoverTime));
                    }
                    else
                    {
                        entryTParam += EntrySpeed * Time.smoothDeltaTime;
                        transform.position = EntryPath.GetPointOnCurve(entryTParam);
                    }

                    break;
                case DroneState.Moving:
                    if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
                    {
                        StartCoroutine(OnHover(HoverTime));
                    }
                    else
                    {
                        var step = Speed * Time.smoothDeltaTime;
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
                    }

                    break;
                case DroneState.Hovering:
                    break;
                case DroneState.Shooting:
                    bulletManager.Spawn(transform.position, bulletManager.Plane == BulletPlane.XY ? transform.up : transform.forward);
                    break;
                case DroneState.Dying:
                default:
                    break;
            }
        }

        private IEnumerator OnHover(float hoverTime)
        {
            State = DroneState.Hovering;
            var timer = 0.0f;

            while (timer < hoverTime)
            {
                timer += Time.smoothDeltaTime;
                yield return null;
            }

            // If this drone is part of a group, invoke entry complete
            if (!IsEntryComplete)
            {
                EntryCompleteEvent?.Invoke(Group);
                IsEntryComplete = true;
            }
            else
            {
                State = DroneState.Shooting;
            }

            targetPosition = PathController.GetNextPoint();
        }

        // Called from Bullet Manager OnBulletSpawned event
        public void OnBulletSpawned(int something, BulletContainer bullet)
        {
            currentBulletIndex++;

            if (currentBulletIndex >= MaxBullets - 1)
            {
                currentBulletIndex = 0;
                State = DroneState.Moving;
            }
        }

        // Called from EnemyHealth
        public IEnumerator PlayDeath()
        {
            State = DroneState.Dying;

            // Broadcast that death has happened
            DeathEvent?.Invoke(Group, PointValue);

            // Run IEnumerator to spawn explosions
            for (int i = 0; i < NumberOfExplosions; i++)
            {
                StartCoroutine(AsyncPlayDeath());
                // yield return new WaitForSeconds(0.025f);
                yield return new WaitUntil(() => CanDestroy);
            }

            // Destroy this object
            GameObject.Destroy(gameObject);
        }

        private IEnumerator AsyncPlayDeath()
        {
            Vector3 scaleMin = Vector3.one;
            Vector3 scaleMax = Vector3.one;
            float minRandomRange = 0.25f;
            float maxRandomRange = 0.5f;
            float timeToShrink = 0.5f;

            var pos = transform.position;

            var mesh = ExplosionPrefab.GetComponent<Mesh>();
            var material = ExplosionPrefab.GetComponent<Material>();

            var randomScale = Random.Range(0f, 1f);
            var scale = Vector3.Lerp(scaleMax, scaleMin, randomScale);

            var random = Random.onUnitSphere * Random.Range(minRandomRange, maxRandomRange);

            var timer = 0.0f;

            while (timer < timeToShrink)
            {
                var t = timer / timeToShrink;

                Graphics.DrawMesh(
                    ExplosionMesh,
                    Matrix4x4.TRS(pos + random, Quaternion.identity, Vector3.Lerp(scale, Vector3.zero, t*t*t)),
                    ExplosionMaterial,
                    LayerMask.NameToLayer("Default")
                );
                
                timer += Time.deltaTime;

                yield return null;
            }

            CanDestroy = true;
        }
    }
}
