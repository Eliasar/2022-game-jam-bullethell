using UnityEngine;

namespace Confined
{
    public class RotateRotors : MonoBehaviour
    {
        [SerializeField] private float RotationsPerSecond = 1.0f;

        private void Update()
        {
            transform.Rotate(new Vector3(0, RotationsPerSecond * 360, 0) * Time.smoothDeltaTime);
        }
    }
}