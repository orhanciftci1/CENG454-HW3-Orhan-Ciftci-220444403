using UnityEngine;

namespace CoreBreach.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private Camera mainCamera;

        private Rigidbody2D body;
        private Vector2 movement;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            AimAtMouse();
        }

        private void FixedUpdate()
        {
            body.MovePosition(body.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        private void AimAtMouse()
        {
            if (mainCamera == null)
            {
                return;
            }

            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 aim = mouseWorld - transform.position;
            if (aim.sqrMagnitude > 0.01f)
            {
                transform.right = aim.normalized;
            }
        }
    }
}
