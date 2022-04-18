using UnityEngine;

namespace Confined
{
    public class BezierPath : MonoBehaviour
    {
        [SerializeField] private Transform[] ControlPoints;
        [SerializeField] private float GizmoStepSize = 0.05f;
        [SerializeField] private bool DrawPath = true;

        private Vector2 gizmosPosition;

        public Vector2 GetPointOnCurve(float t)
        {
            return
                Mathf.Pow(1 - t, 3) * ControlPoints[0].localPosition +
                3 * Mathf.Pow(1 - t, 2) * t * ControlPoints[1].localPosition + 
                3 * (1 - t) * Mathf.Pow(t, 2) * ControlPoints[2].localPosition + 
                Mathf.Pow(t, 3) * ControlPoints[3].localPosition;
        }

        private void OnDrawGizmos()
        {
            if (!DrawPath)
            {
                return;
            }

            Gizmos.color = Color.white;
            for (float t = 0; t <= 1; t += GizmoStepSize)
            {
                Gizmos.DrawSphere(GetPointOnCurve(t), 0.25f);
            }

            var p0 = ControlPoints[0].localPosition;
            var p1 = ControlPoints[1].localPosition;
            var p2 = ControlPoints[2].localPosition;
            var p3 = ControlPoints[3].localPosition;

            // Draw spheres at p0, p1, p2, p3
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(new Vector3(p0.x, p0.y, 0.0f), 0.4f);
            Gizmos.DrawSphere(new Vector3(p1.x, p1.y, 0.0f), 0.4f);
            Gizmos.DrawSphere(new Vector3(p2.x, p2.y, 0.0f), 0.4f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(new Vector3(p3.x, p3.y, 0.0f), 0.4f);

            // Draw lines between p0, p1 and p2, p3
            Gizmos.color = Color.white;
            Gizmos.DrawLine(new Vector2(p0.x, p0.y), new Vector2(p1.x, p1.y));
            Gizmos.DrawLine(new Vector2(p2.x, p2.y), new Vector2(p3.x, p3.y));
        }
    }
}
