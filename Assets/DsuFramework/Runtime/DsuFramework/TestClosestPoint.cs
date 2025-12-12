using UnityEngine;
using Dsu.Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestClosestPoints : MonoBehaviour
{
    [Header("Line 1")]
    public Vector3 line1Start = new Vector3(-3, 1, 0);
    public Vector3 line1Direction = Vector3.right;

    [Header("Line 2")]
    public Vector3 line2Start = new Vector3(0, -2, 2);
    public Vector3 line2Direction = Vector3.up;

    private void OnDrawGizmos()
    {
        // Validate: line direction must not be zero
        if (line1Direction == Vector3.zero || line2Direction == Vector3.zero) {
            Debug.LogWarning("Line direction cannot be zero. Please assign valid direction vectors.");
            return;
        }

        // Normalize directions and scale for visualization
        Vector3 dir1 = line1Direction.normalized * 10f;
        Vector3 dir2 = line2Direction.normalized * 10f;

        // Draw Line 1 in blue
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(line1Start - dir1, line1Start + dir1);

        // Draw Line 2 in white
        Gizmos.color = Color.white;
        Gizmos.DrawLine(line2Start - dir2, line2Start + dir2);

#if UNITY_EDITOR
        // Show labels in Scene View
        Handles.Label(line1Start, "Line 1 Start", EditorStyles.boldLabel);
        Handles.Label(line2Start, "Line 2 Start", EditorStyles.boldLabel);
#endif

        // Compute closest points
        if (Math3D.ClosestPointsOnTwoLines(out Vector3 pointOnLine1, out Vector3 pointOnLine2,
            line1Start, line1Direction, line2Start, line2Direction)) {
            // Draw red line between closest points
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointOnLine1, pointOnLine2);

            // Draw small spheres at the closest points
            Gizmos.DrawSphere(pointOnLine1, 0.05f);
            Gizmos.DrawSphere(pointOnLine2, 0.05f);

#if UNITY_EDITOR
            // Show distance label between the points
            Vector3 midPoint = (pointOnLine1 + pointOnLine2) * 0.5f;
            float distance = Vector3.Distance(pointOnLine1, pointOnLine2);
            Handles.Label(midPoint, "Distance: " + distance.ToString("F3"), EditorStyles.whiteLabel);
#endif
        }
    }
}
