using UnityEngine;

namespace Dsu.Framework
{
    public class SimpleCameraFollow : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform followTarget;
        [SerializeField] private float smoothTime = 0.3f;

        private Vector3 velocity = Vector3.zero;

        private void Start()
        {
            if (followTarget == null) return;

            Vector3 initialPos = followTarget.position;
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

            transform.position = smoothPos;
        }
    }
}