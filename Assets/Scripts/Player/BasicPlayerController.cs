using UnityEngine;

namespace Confined
{
    public class BasicPlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        
        private float h, v;

        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            // Movement
            // h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            // Move
            if (h != 0.0f || v != 0.0f)
            {
                Move(h, v);
            }
        }

        private void Move(float horizontal, float vertical)
        {
            transform.Translate(Vector3.right * horizontal * Time.smoothDeltaTime * speed);
            transform.Translate(Vector3.up * vertical * Time.smoothDeltaTime * speed);
        }
    }
}
