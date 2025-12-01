using UnityEngine;
using UnityEngine.InputSystem;

namespace eranboi.EquipmentSystem.Demo
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float turnSpeed = 720f;
        
        private CharacterController characterController;
        private Vector2 moveInput;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            HandleInput();
            Move();
        }

        private void HandleInput()
        {
            var _left = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1f : 0f;
            var _right = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f;
            var _up = Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1f : 0f;
            var _down = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1f : 0f;

            moveInput = new Vector2(_left + _right, _up + _down).normalized;
        }

        private void Move()
        {
            if (moveInput.magnitude < 0.1f) return;

            // Calculate direction relative to camera or just world space
            var _direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            // Rotate
            var _targetRotation = Quaternion.LookRotation(_direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, turnSpeed * Time.deltaTime);

            // Move
            characterController.Move(_direction * moveSpeed * Time.deltaTime);
        }
    }
}