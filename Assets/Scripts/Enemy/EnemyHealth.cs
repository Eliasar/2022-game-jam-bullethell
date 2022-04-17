using BulletFury;
using BulletFury.Data;
using Ktyl.Util;
using UnityEngine;

namespace Confined
{
    public class EnemyHealth : MonoBehaviour
    {
        public SerialFloat health;
        [SerializeField] private float localHealth;

        public float DoNotTouch;

        // Generalize DroneXWB reference later
        [SerializeField] private DroneXWB droneScript;

        // Property for overall health access
        public float Health
        {
            get
            {
                if (health?.Value != null)
                {
                    return health.Value;
                }
                else
                {
                    return localHealth;
                }
            }

            set
            {
                if (health?.Value != null)
                {
                    health.Value = value;
                }
                else
                {
                    localHealth = value;
                }
            }
        }

        private void Update()
        {
            DoNotTouch = Health;
        }

        public void OnCollide(BulletContainer bullet, BulletCollider collider)
        {
            // Get the bullet damage
            float damage = bullet.Damage;

            // Subtract from current health
            collider.GetComponent<EnemyHealth>().Health -= damage;

            // Die if below 0
            if (Health <= 0 && droneScript != null && droneScript.IsDying == false)
            {
                StartCoroutine(droneScript.PlayDeath());
            }
        }
    }
}