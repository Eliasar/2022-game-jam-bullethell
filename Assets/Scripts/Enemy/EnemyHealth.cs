using BulletFury;
using BulletFury.Data;
using UnityEngine;

namespace Confined
{
    public class EnemyHealth : MonoBehaviour
    {
        // public SerialFloat health;
        [SerializeField] private float health;

        // Generalize DroneXWB reference later
        [SerializeField] private DroneXWB droneScript;

        // Property for overall health access
        public float Health
        {
            get => health;       
            set => health = value;
        }

        // Called from the BulletManager
        public void OnCollide(BulletContainer bullet, BulletCollider collider)
        {
            // Get the bullet damage
            float damage = bullet.Damage;

            // Subtract from current health
            collider.GetComponent<EnemyHealth>().Health -= damage;

            // Die if below 0
            if (Health <= 0 && droneScript != null && droneScript.State != DroneXWB.DroneState.Dying)
            {
                StartCoroutine(droneScript.PlayDeath());
            }
        }
    }
}