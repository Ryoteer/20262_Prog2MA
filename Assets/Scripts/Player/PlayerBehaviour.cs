using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("<color=green>Animation</color>")]
    [SerializeField] private string _xAxisName = "xAxis";
    [SerializeField] private string _yAxisName = "yAxis";
    [SerializeField] private string _jumpTriggerName = "onJump";
    [SerializeField] private string _landTriggerName = "onLanding";
    [SerializeField] private string _airBoolName = "isOnAir";
    [SerializeField] private string _groundBoolName = "isGrounded";

    [Header("<color=green>Physics</color>")]
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _moveSpeed = 3.5f;

    private bool _isOnAir = false;

    private Animator _animator;
    private PlayerInputAction _inputAction;
    private Rigidbody _rb;

    private Vector2 _rawInput = new();
    private Vector3 _dir = new();

    private void Awake()
    {
        _inputAction = new PlayerInputAction();
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
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
        _animator.SetFloat(_xAxisName, _rawInput.x);
        _animator.SetFloat(_yAxisName, _rawInput.y);
        _animator.SetBool(_airBoolName, _isOnAir);
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
        if (!_isOnAir)
        {
            _animator.SetTrigger(_jumpTriggerName);

            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

            _isOnAir = true;
        }
    }

    private void Movement(Vector2 input)
    {
        _dir = (transform.right * input.x + transform.forward * input.y).normalized;        

        _rb.MovePosition(transform.position + _dir * _moveSpeed * Time.fixedDeltaTime);

        #region Physics movement
        //_rb.linearVelocity = new Vector3(input.x * _moveSpeed, 0.0f, input.y * _moveSpeed);

        //_dir = (Vector3.right * input.y + Vector3.forward * -input.x);

        //_rb.AddTorque(_dir * _moveSpeed, ForceMode.Force);
        #endregion
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 30 && _isOnAir)
        {
            _animator.SetTrigger(_landTriggerName);

            _isOnAir = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 30 && !_isOnAir)
        {
            _isOnAir = true;
        }
    }
}
