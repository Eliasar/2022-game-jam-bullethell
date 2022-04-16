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

        private void Update()
        {
            DoNotTouch = health.Value;
        }

        public void OnCollide(BulletContainer bullet, BulletCollider collider)
        {
            // Get the bullet damage
            float damage = bullet.Damage;

            // Subtract from current health
            collider.GetComponent<PlayerHealth>().health.Value -= damage;

            // If below 0, I don't know, fucking we'll do something, eh?
            if (health.Value <= 0)
            {
                Debug.Log("Umm, you're dead dude");
            }
        }
    }
}