using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("<color=green>Physics</color>")]
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _moveSpeed = 3.5f;

    private PlayerInputAction _inputAction;
    private Rigidbody _rb;

    private Vector2 _rawInput = new();
    private Vector3 _dir = new();

    private void Awake()
    {
        _inputAction = new PlayerInputAction();
        _rb = GetComponent<Rigidbody>();
    }

    #region Input Actions
    private void OnEnable()
    {
        _inputAction.Enable();
        _inputAction.Player.Jump.performed += JumpAction;
        _inputAction.Player.Movement.performed += MoveAction;
        _inputAction.Player.Movement.canceled += MoveCancel;
    }

    private void OnDisable()
    {
        _inputAction.Disable();
        _inputAction.Player.Jump.performed -= JumpAction;
        _inputAction.Player.Movement.performed -= MoveAction;
        _inputAction.Player.Movement.canceled -= MoveCancel;
    }

    private void MoveAction(InputAction.CallbackContext value)
    {
        _rawInput = value.ReadValue<Vector2>();
    }

    private void MoveCancel(InputAction.CallbackContext value)
    {
        _rawInput = Vector2.zero;
    }

    private void JumpAction(InputAction.CallbackContext value)
    {
        Jump();
    }
    #endregion

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (_rawInput.sqrMagnitude != 0.0f)
        {
            Movement(_rawInput);
        }
    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void Movement(Vector2 input)
    {
        _dir = (transform.right * input.x + transform.forward * input.y).normalized;

        _rb.MovePosition(transform.position + _dir * _moveSpeed * Time.fixedDeltaTime);
    }
}
