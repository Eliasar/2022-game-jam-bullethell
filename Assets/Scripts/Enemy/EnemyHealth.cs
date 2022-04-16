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

            // If below 0, I don't know, fucking we'll do something, eh?
            if (Health <= 0)
            {
                Debug.Log("Umm, you're dead dude");
            }
        }
    }
}