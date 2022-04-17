using BulletFury;
using BulletFury.Data;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Confined
{
    public class DroneXWB : MonoBehaviour
    {
        [Header("Position")]
        [SerializeField] private float Speed = 15.0f;
        [SerializeField] private float HoverTime = 2.0f;
        [SerializeField] private Vector3[] AttackPositions;
        private int currentPositionIndex;
        private Vector3 targetPosition;
        private bool IsHovering = false;

        [Header("Bullets")]
        [SerializeField] private BulletManager bulletManager = null;
        [SerializeField] private int MaxBullets = 5;
        [SerializeField] private Transform PlayerTransform = null;
        private int currentBulletIndex;
        private bool CanFire = false;

        [Header("Death")]
        [SerializeField] private GameObject ExplosionPrefab = null;
        [SerializeField] private Mesh ExplosionMesh;
        [SerializeField] private Material ExplosionMaterial;
        public bool IsDying = false;

        // private void Awake()
        // {
        //     Application.targetFrameRate = 120;
        // }

        private void Start()
        {
            currentPositionIndex = 0;
            currentBulletIndex = 0;
            targetPosition = AttackPositions[currentPositionIndex];
        }

        // Update is called once per frame
        void Update()
        {
            if (bulletManager == null || IsDying)
            {
                return;
            }

            // Look at the player
            transform.up = PlayerTransform.position - transform.position;

            // Once it gets close to its target position, hover for a time, then set CanFire
            // Move, hover, or fire, those are your two choices
            if (!IsHovering && Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                StartCoroutine(OnHover());
            }
            else if (CanFire)
            {
                bulletManager.Spawn(transform.position, bulletManager.Plane == BulletPlane.XY ? transform.up : transform.forward);
            }
            else
            {
                // Move to the next target position
                var step = Speed * Time.smoothDeltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            }
        }

        private IEnumerator OnHover()
        {
            IsHovering = true;
            var timer = 0.0f;

            while (timer < HoverTime)
            {
                timer += Time.smoothDeltaTime;

                yield return null;
            }

            currentPositionIndex = (currentPositionIndex + 1) % AttackPositions.Length;
            targetPosition = AttackPositions[currentPositionIndex];

            IsHovering = false;
            CanFire = true;
        }

        public void OnBulletSpawned(int something, BulletContainer bullet)
        {
            currentBulletIndex++;

            if (currentBulletIndex > MaxBullets)
            {
                CanFire = false;
                currentBulletIndex = 0;
            }
        }

        public IEnumerator PlayDeath()
        {
            IsDying = true;
            int numberOfExplosions = 30;

            // Run IEnumerator to spawn explosions
            for (int i = 0; i < numberOfExplosions; i++)
            {
                StartCoroutine(AsyncPlayDeath());
                yield return new WaitForSeconds(0.025f);
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
        }
    }
}
