using UnityEngine;

namespace Dsu.Framework
{
    public class Simple2DPlatformerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;

        [Header("Jump Settings")]
        public float jumpHeight = 1f;
        public LayerMask groundLayer;
        public Transform groundCheck;
        public Vector2 groundCheckBoxSize = new Vector2(0.5f, 0.1f); // Box size for ground check

        [Header("Gravity Settings")]
        public float gravityScale = 3f;
        public float maxFallSpeed = 10f;

        [Header("Trajectory Prediction Settings")]
        public bool showTrajectory = true;
        public float predictionTime = 1.5f;
        public int predictionSteps = 30;
        public Color trajectoryColor = Color.cyan;

        private Rigidbody2D rb;
        private Vector2 velocity;
        private Vector2 movementInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // gravity is handled manually
        }

        private void Update()
        {
            movementInput.x = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) {
                float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y * gravityScale) * jumpHeight);
                velocity.y = jumpVelocity;
            }
        }

        private void FixedUpdate()
        {
            // Apply custom gravity
            velocity.y += Physics2D.gravity.y * gravityScale * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);

            // Horizontal movement
            velocity.x = movementInput.x * moveSpeed;

            // Move the character
            Vector2 nextPosition = rb.position + velocity * Time.fixedDeltaTime;
            rb.MovePosition(nextPosition);

            // Stop falling if grounded
            if (IsGrounded() && velocity.y < 0) {
                velocity.y = 0f;
            }
        }

        private bool IsGrounded()
        {
            if (groundCheck == null) {
                Debug.LogWarning("GroundCheck Transform is not assigned.");
                return false;
            }

            Collider2D hit = Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0f, groundLayer);
            return hit != null;
        }

        private void OnDrawGizmos()
        {
            if (groundCheck != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(groundCheck.position, groundCheckBoxSize);
            }

#if UNITY_EDITOR
            if (!Application.isPlaying || !showTrajectory) return;

            Vector2 simulatedPosition = rb.position;
            Vector2 simulatedVelocity = velocity;
            float stepTime = predictionTime / predictionSteps;
            float gravityY = Physics2D.gravity.y * gravityScale;

            Gizmos.color = trajectoryColor;
            Vector3 prevPoint = simulatedPosition;

            for (int i = 0; i < predictionSteps; i++) {
                // Simulate physics
                simulatedVelocity.y += gravityY * stepTime;
                simulatedVelocity.y = Mathf.Max(simulatedVelocity.y, -maxFallSpeed);
                simulatedVelocity.x = movementInput.x * moveSpeed;

                simulatedPosition += simulatedVelocity * stepTime;

                Vector3 nextPoint = simulatedPosition;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
#endif
        }
    }
}