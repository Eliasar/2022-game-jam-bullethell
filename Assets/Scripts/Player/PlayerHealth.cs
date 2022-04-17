using BulletFury;
using BulletFury.Data;
using Ktyl.Util;
using UnityEngine;

namespace Confined
{
    public class PlayerHealth : MonoBehaviour
    {
        public SerialFloat health;
        public float DoNotTouch;

        // Property for overall health access
        public float Health
        {
            get
            {
                return health.Value;
            }

            set
            {
                health.Value = value;
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
            collider.GetComponent<PlayerHealth>().Health -= damage;

            // If below 0, I don't know, fucking we'll do something, eh?
            if (Health <= 0)
            {
                Debug.Log("Umm, you're dead dude");
            }
        }
    }
}