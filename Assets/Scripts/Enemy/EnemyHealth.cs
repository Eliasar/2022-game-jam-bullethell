using BulletFury;
using BulletFury.Data;
using Confined.Enemies;
using UnityEngine;

namespace Confined
{
    public class EnemyHealth : MonoBehaviour
    {
        // public SerialFloat health;
        [SerializeField] private float health;

        // Generalize DroneXWB reference later
        [SerializeField] private DroneXwb droneScript;

        // Property for overall health access
        public float Health
        {
            get => health;
            set => health = value;
        }

        // Called from the BulletManager
        public void OnCollide(BulletContainer bullet, BulletCollider collider)
        {
            if (Health > 0)
            {
                // Subtract from current health
                Health -= bullet.Damage;
            }

            // Die if below 0
            if (Health <= 0 && droneScript?.State != DroneXwb.DroneState.Dying)
            {
                StartCoroutine(droneScript.PlayDeath());
                GetComponent<BulletCollider>().enabled = false;
            }
        }
    }
}