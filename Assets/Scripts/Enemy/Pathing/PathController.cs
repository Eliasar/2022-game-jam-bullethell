using UnityEngine;

namespace Confined
{
    public class PathController : MonoBehaviour
    {
        // Current index
        private int currentIndex = 0;

        // Debug
        [SerializeField] private bool DrawPaths = false;

        public Vector3 GetFirstPoint()
        {
            currentIndex = 0;
            return transform.GetChild(currentIndex).transform.localPosition;
        }

        // When told to get the next one, gather your children, and feed the caller the next one
        // That got dark...
        public Vector3 GetNextPoint()
        {
            currentIndex = (currentIndex + 1) % transform.childCount;
            return transform.GetChild(currentIndex).transform.localPosition;
        }

        private void OnDrawGizmos()
        {
            if (!DrawPaths)
            {
                return;
            }

            Vector3 currentDebugPosition = transform.GetChild(0).localPosition;
            Gizmos.color = Color.cyan;

            for (int i = 0; i < transform.childCount; i++)
            {
                Vector3 nextDebugPosition = transform.GetChild(i).localPosition;
                Debug.DrawLine(currentDebugPosition, nextDebugPosition);
                currentDebugPosition = nextDebugPosition;
                Gizmos.DrawSphere(transform.GetChild(i).localPosition, 0.3f);
            }

            Debug.DrawLine(currentDebugPosition, transform.GetChild(0).localPosition);

        }
    }
}
