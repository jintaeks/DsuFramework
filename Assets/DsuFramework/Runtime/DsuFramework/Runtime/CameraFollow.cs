using UnityEngine;

namespace Dsu.Framework
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform followTarget;
        [SerializeField] private float smoothTime = 0.3f;

        [Header("Camera Bounds (Collider2D)")]
        [SerializeField] private Collider2D cameraBounds;
        [SerializeField] private bool showBoundsGizmo = true;

        private Vector3 velocity = Vector3.zero;
        private Vector2 minBounds;
        private Vector2 maxBounds;

        private void Start()
        {
            if (followTarget == null) return;

            SetBoundsFromCollider();

            Vector3 initialPos = GetClampedPosition(followTarget.position);
            initialPos.z = transform.position.z; // Maintain the camera's original z position
            transform.position = initialPos;
        }

        private void LateUpdate()
        {
            if (followTarget == null) return;

            Vector3 targetPos = new Vector3(
                followTarget.position.x,
                followTarget.position.y,
                transform.position.z
            );

            Vector3 smoothPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

            Vector3 clampedPos = GetClampedPosition(smoothPos);
            transform.position = clampedPos;
        }

        private void SetBoundsFromCollider()
        {
            if (cameraBounds) {
                Bounds bounds = cameraBounds.bounds;
                minBounds = bounds.min;
                maxBounds = bounds.max;
            }
        }

        private Vector3 GetClampedPosition(Vector3 position)
        {
            if (!cameraBounds) {
                return position;
            }

            float camHalfHeight = Camera.main.orthographicSize;
            float camHalfWidth = camHalfHeight * Camera.main.aspect;

            float clampedX = Mathf.Clamp(position.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
            float clampedY = Mathf.Clamp(position.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);

            return new Vector3(clampedX, clampedY, position.z);
        }

        private void OnDrawGizmos()
        {
            if (!showBoundsGizmo || cameraBounds == null) return;

            Gizmos.color = Color.yellow;
            Bounds bounds = cameraBounds.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}