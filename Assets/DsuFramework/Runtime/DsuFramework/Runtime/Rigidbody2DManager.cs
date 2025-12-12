using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dsu.Framework
{
    public class Rigidbody2DManager : MonoBehaviour
    {
        public bool showLog = false; // for debug
        public bool autoCollectOnStart = true;
        [Tooltip("GameObjects with Rigidbody components. Only one will be active at a time.")]
        public List<GameObject> rigidbodyObjects = new List<GameObject>();
        private List<Rigidbody2D> managedRigidbodies = new List<Rigidbody2D>();
        private Rigidbody2D activeRigidbody;

        [Header("Debug Visualization")]
        public bool showDebugInfo = true; // Show debug info in the scene view
        public bool showRigidbody2DInfo = false;
        [Tooltip("Color used to highlight managed Rigidbody2D components in the scene view")]
        public Color highlightColor = new Color(0, 1, 0, 0.3f); // Semi-transparent green
        [Tooltip("Color used to highlight the active Rigidbody2D")]
        public Color activeHighlightColor = new Color(1, 1, 0, 0.3f); // Semi-transparent red
        [Tooltip("Size of the highlight box relative to the object's bounds")]
        public float highlightSize = 1.1f;

        private void Awake()
        {
            if( autoCollectOnStart )
                FindAndRegisterChildAgents();
        }

        /// <summary>
        /// Finds all immediate child GameObjects with a Rigidbody2DAgent component
        /// and adds them to the rigidbodyObjects list.
        /// Existing list will be cleared first.
        /// </summary>
        public void FindAndRegisterChildAgents()
        {
            rigidbodyObjects.Clear();

            foreach (Transform child in transform) {
                if (child == null) continue;

                Rigidbody2DAgent agent = child.GetComponent<Rigidbody2DAgent>();
                if (agent != null) {
                    rigidbodyObjects.Add(child.gameObject);
                }
            }

            RefreshManagedRigidbodies();
        }

#if UNITY_EDITOR
        [ContextMenu("Auto Collect Rigidbody 2D Agents")]
        private void ContextMenu_AutoCollect()
        {
            Undo.RecordObject(this, "Auto Collect Rigidbody 2D Agents");
            FindAndRegisterChildAgents();
            EditorUtility.SetDirty(this);
            Debug.Log("Rigidbody2DManager: Auto-collected Rigidbody2DAgent children.");
        }
#endif

        /// <summary>
        /// Rebuild internal Rigidbody2D list from GameObjects.
        /// </summary>
        private void RefreshManagedRigidbodies()
        {
            managedRigidbodies.Clear();
            foreach (var obj in rigidbodyObjects) {
                if (obj == null) continue;

                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null && !managedRigidbodies.Contains(rb)) {
                    managedRigidbodies.Add(rb);
                }
                else {
#if UNITY_EDITOR
                    if(showLog)
                        Log.Warn($"GameObject '{obj.name}' has no Rigidbody component.");
#endif
                }
            }
        }

        /// <summary>
        /// Activate the specified Rigidbody by enabling its physics simulation,
        /// and disabling it on all others.
        /// </summary>
        public void ActivateRigidbody(Rigidbody2D target)
        {
            if (target == null) {
                if (showLog)
                    Log.Warn($"ActivateRigidbody called with null target.");
                return;
            }

            activeRigidbody = target;

            foreach (Rigidbody2D rb in managedRigidbodies) {
                if (rb == null) continue;
                rb.isKinematic = (rb != target);
                if (rb.isKinematic == true) {
                    rb.SetLinearVelocity(Vector2.zero);
                }
            }

#if UNITY_EDITOR
            if (showLog)
                Log.Info($"Activated Rigidbody: {target.name}");
#endif
        }

        /// <summary>
        /// Clear all managed GameObjects.
        /// </summary>
        public void Clear()
        {
            rigidbodyObjects.Clear();
            managedRigidbodies.Clear();
        }

        private void OnDrawGizmos()
        {
            if (!showDebugInfo)
                return;

            if (!Application.isPlaying) {
                RefreshManagedRigidbodies();
            }

            Gizmos.color = new Color(0, 0, 1, 0.8f); // transparent blue
            Gizmos.DrawSphere(transform.position, 0.3f); // Origin marker

            foreach (var rb in managedRigidbodies) {
                if (rb == null) continue;

                // Determine bounds
                Bounds bounds;
                Collider2D collider = rb.GetComponent<Collider2D>();
                if (collider != null) {
                    bounds = collider.bounds;
                }
                else {
                    Renderer renderer = rb.GetComponent<Renderer>();
                    if (renderer != null) {
                        bounds = renderer.bounds;
                    }
                    else {
                        bounds = new Bounds(rb.transform.position, Vector3.one);
                    }
                }

                bool isActiveRigidbody = (rb == activeRigidbody);
                Vector3 center = bounds.center;
                Vector3 size = bounds.size * highlightSize;

                // Draw filled cube
                Gizmos.color = isActiveRigidbody ? activeHighlightColor : highlightColor;
                Gizmos.DrawCube(center, size);

                // L-shaped dotted line: from origin to horizontal, then to object
                Vector3 horizontalEnd = new Vector3(center.x, transform.position.y, transform.position.z);
                Vector3 verticalEnd = center;

#if UNITY_EDITOR
                UnityEditor.Handles.color = Color.white;
                UnityEditor.Handles.DrawDottedLine(transform.position, horizontalEnd, 2f);
                UnityEditor.Handles.DrawDottedLine(horizontalEnd, verticalEnd, 2f);
#else
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, horizontalEnd);
        Gizmos.DrawLine(horizontalEnd, verticalEnd);
#endif

                // Center marker
                Gizmos.color = new Color(0, 0, 1, 0.5f);
                Gizmos.DrawSphere(center, 0.2f);

                // Outline box if active
                if (isActiveRigidbody) {
                    Gizmos.color = Color.yellow;
                    Vector3 extents = size * 0.5f;
                    Vector3 topLeft = center + new Vector3(-extents.x, extents.y, 0);
                    Vector3 topRight = center + new Vector3(extents.x, extents.y, 0);
                    Vector3 bottomLeft = center + new Vector3(-extents.x, -extents.y, 0);
                    Vector3 bottomRight = center + new Vector3(extents.x, -extents.y, 0);

                    Gizmos.DrawLine(topLeft, topRight);
                    Gizmos.DrawLine(topRight, bottomRight);
                    Gizmos.DrawLine(bottomRight, bottomLeft);
                    Gizmos.DrawLine(bottomLeft, topLeft);
                }

#if UNITY_EDITOR
                if (showRigidbody2DInfo) {
                    Vector3 labelPosition = center + Vector3.up * (size.y * 0.5f + 0.2f);
                    string infoText = $"Kinematic: {rb.isKinematic}, Body Type: {rb.bodyType}";

                    UnityEditor.Handles.Label(labelPosition, infoText, new GUIStyle()
                    {
                        normal = { textColor = Color.white },
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 12
                    });
                }

                // Optional: Draw name label above Rigidbody
                //Vector3 nameLabelPosition = center + Vector3.up * (size.y * 0.5f + 0.5f);
                //DrawBackgroundLabel(nameLabelPosition, rb.gameObject.name);
#endif
            }
        }
    }
}