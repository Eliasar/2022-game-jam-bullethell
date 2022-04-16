using BulletFury;
using BulletFury.Data;
using UnityEngine;

public class DroneXWB : MonoBehaviour
{
    [SerializeField] private float Speed = 1.0f;

    [SerializeField] private BulletManager bulletManager = null;

    [SerializeField] private Vector3[] AttackPositions;

    private int currentIndex;
    private Vector3 target;
    private bool CanFire = false;

    private int currentBulletIndex;
    [SerializeField] private int MaxBullets = 5;

    [SerializeField] private Transform PlayerTransform;

    private void Awake()
    {
        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        currentIndex = 0;
        currentBulletIndex = 0;
        target = AttackPositions[currentIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (bulletManager == null)
        {
            return;
        }

        // Look at the player
        transform.up = PlayerTransform.position - transform.position;

        // Check if the position is approximate to the target
        if (!CanFire && Vector3.Distance(transform.position, target) < 0.001f)
        {
            currentIndex = (currentIndex + 1) % AttackPositions.Length;
            target = AttackPositions[currentIndex];
            CanFire = true;
        }

        // Move or fire, those are your two choices
        if (CanFire)
        {
            bulletManager.Spawn(transform.position, bulletManager.Plane == BulletPlane.XY ? transform.up : transform.forward);
        }
        else
        {
            // Move to the next target position
            var step = Speed * Time.smoothDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
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
}
