using UnityEngine;

using BulletFury;
using System.Collections.Generic;
using System.Linq;
using BulletFury.Data;

namespace Confined
{
    public class BasicPlayerController : MonoBehaviour
    {
        [SerializeField] private float MovementSpeed = 5f;

        private float h, v;

        // Rotation Object
        [SerializeField] private Transform RotationObject;

        // Shield properties
        [SerializeField] private float ShieldSize = 1f;
        [SerializeField] private float ShieldSpeed = 1f;

        // Turret properties
        [SerializeField] private List<BulletManager> Turrets;

        private void Awake()
        {
            Application.targetFrameRate = 120;
        }

        void Start()
        {
            // Gather all turrets from child components
            Turrets = GetComponentsInChildren<BulletManager>().ToList();
        }

        // Update is called once per frame
        void Update()
        {
            // Movement
            v = Input.GetAxisRaw("Vertical");

            // Move
            if (h != 0.0f || v != 0.0f)
            {
                Move(h, v);
            }

            // Shield and Turret rotation
            var rotatorDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(RotationObject.transform.position);
            var rotatorAngle = Mathf.Atan2(rotatorDirection.y, rotatorDirection.x) * Mathf.Rad2Deg;
            RotationObject.rotation = Quaternion.AngleAxis(rotatorAngle, Vector3.forward);

            // Turret Firing
            if (Input.GetMouseButton(0))
            {
                foreach (var turret in Turrets)
                {
                    turret.Spawn(turret.transform.position,
                        turret.Plane == BulletPlane.XY ? turret.transform.up : turret.transform.forward);
                }
            }
        }

        private void Move(float horizontal, float vertical)
        {
            transform.Translate(Vector3.right * horizontal * Time.smoothDeltaTime * MovementSpeed);
            transform.Translate(Vector3.up * vertical * Time.smoothDeltaTime * MovementSpeed);
        }
    }
}
