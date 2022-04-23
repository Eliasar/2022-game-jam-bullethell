using BulletFury;
using BulletFury.Data;
using Ktyl.Util;
using UnityEngine;

namespace Confined
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private SerialFloat health;
        [SerializeField] private float enumeratedHealth;

        public void Update()
        {
            enumeratedHealth = health.Value;
        }

        public void OnCollide(BulletContainer bullet, BulletCollider collider)
        {
            // Debug collision info
            Debug.Log($"Bullet: [{bullet.Position}]; Collider: [{collider.}]");

            // Subtract from current health
            health.Value -= bullet.Damage;

            // If below 0, I don't know, fucking we'll do something, eh?
            if (health.Value <= 0)
            {
                Debug.Log("Umm, you're dead dude");
            }
        }
    }
}