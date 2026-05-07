using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

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
            movement = ReadMovement();
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

            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(ReadPointerPosition());
            Vector2 aim = mouseWorld - transform.position;
            if (aim.sqrMagnitude > 0.01f)
            {
                transform.right = aim.normalized;
            }
        }

        private Vector2 ReadMovement()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                Vector2 input = Vector2.zero;
                input.x = ReadKey(Keyboard.current.dKey, Keyboard.current.rightArrowKey)
                    - ReadKey(Keyboard.current.aKey, Keyboard.current.leftArrowKey);
                input.y = ReadKey(Keyboard.current.wKey, Keyboard.current.upArrowKey)
                    - ReadKey(Keyboard.current.sKey, Keyboard.current.downArrowKey);
                return input.normalized;
            }
#endif
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }

        private Vector3 ReadPointerPosition()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                Vector2 position = Mouse.current.position.ReadValue();
                return new Vector3(position.x, position.y, 0f);
            }
#endif
            return Input.mousePosition;
        }

#if ENABLE_INPUT_SYSTEM
        private static float ReadKey(KeyControl primary, KeyControl secondary)
        {
            return primary.isPressed || secondary.isPressed ? 1f : 0f;
        }
#endif
    }
}
