using UnityEngine;

namespace Dsu.Framework
{
    public class Rigidbody2DAgent : MonoBehaviour
    {
        public bool showLog = false; // for debug
        private Rigidbody2DManager rigidbody2DManager;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            Transform current = transform;
            while (current != null) {
                rigidbody2DManager = current.GetComponent<Rigidbody2DManager>();
                if (rigidbody2DManager != null) {
                    break;
                }
                current = current.parent;
            }

            if (rigidbody2DManager == null) {
                if(showLog)
                    Log.Info($"Rigidbody2DManager not found on GameManager.");
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryActivateIfPlayerTouched(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TryActivateIfPlayerTouched(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            // Exit 시에는 아무 동작하지 않음
        }

        private void TryActivateIfPlayerTouched(Collision2D collision)
        {
            if (rigidbody2DManager == null) return;

            if (collision.collider.CompareTag("Player")) {
                rigidbody2DManager.ActivateRigidbody(rb);
                if (showLog)
                    Log.Info( $"Activated self: {gameObject.name}");
            }
        }
    }
}