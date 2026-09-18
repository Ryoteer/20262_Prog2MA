using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : Entity
{
    [Header("<color=green>Animation</color>")]
    [SerializeField] private float _smoothInputSpeed = 0.2f;
    [SerializeField] private string _xAxisName = "xAxis";
    [SerializeField] private string _yAxisName = "yAxis";
    [SerializeField] private string _interactTriggerName = "onInteract";
    [SerializeField] private string _jumpTriggerName = "onJump";
    [SerializeField] private string _landTriggerName = "onLanding";
    [SerializeField] private string _airBoolName = "isOnAir";
    [SerializeField] private string _moveBoolName = "isMoving";
    [SerializeField] private string _groundBoolName = "isGrounded";
    [SerializeField] private string _deathTriggerName = "onDeath";
    [SerializeField] private string _aAttackTriggerName = "onAreaAttack";
    [SerializeField] private string _mAttackTriggerName = "onMeleeAttack";
    [SerializeField] private string _rAttackTriggerName = "onRangeAttack";

    [Header("<color=green>Physics</color>")]
    [SerializeField] private float _areaSphereRadius = 5.0f;
    [SerializeField] private LayerMask _entityRayMask;
    [SerializeField] private float _groundRayLength = 0.125f;
    [SerializeField] private LayerMask _groundRayMask;
    [SerializeField] private float _interactRayLength = 1.5f;
    [SerializeField] private LayerMask _interactableRayMask;
    [SerializeField] private float _interactSphereRadius = 0.625f;
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _meleeRayLength = 1.0f;
    [SerializeField] private float _moveSpeed = 3.5f;
    [SerializeField] private float _rangeRayLength = 30.0f;
    [SerializeField] private float _rangeSphereRadius = 1.0f;

    [Header("<color=green>Stats</color>")]
    [SerializeField] private int _damage = 20;

    private bool _isOnAir = false;

    private Animator _animator;
    private PlayerInputAction _inputAction;
    private Rigidbody _rb;

    private Vector2 _rawInput = new(), _smoothInput = new(), _smoothVelocity = new();
    private Vector3 _dir = new(), _groundRayOffset = new(), _castRayOffset = new();

    private Ray _groundRay, _castRay;
    private RaycastHit _entityHit;

    protected override void Awake()
    {
        base.Awake();

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
        _inputAction.Player.Suicide.performed += SuicideAction;
        _inputAction.Player.Interact.performed += InteractAction;
        _inputAction.Player.Jump.performed += JumpAction;
        _inputAction.Player.AreaAttack.performed += AreaAttackAction;
        _inputAction.Player.MeleeAttack.performed += MeleeAttackAction;
        _inputAction.Player.RangeAttack.performed += RangeAttackAction;
        _inputAction.Player.Movement.performed += MoveAction;
        _inputAction.Player.Movement.canceled += MoveCancel;
    }

    private void OnDisable()
    {
        _inputAction.Disable();
        _inputAction.Player.Suicide.performed -= SuicideAction;
        _inputAction.Player.Interact.performed -= InteractAction;
        _inputAction.Player.Jump.performed -= JumpAction;
        _inputAction.Player.AreaAttack.performed -= AreaAttackAction;
        _inputAction.Player.MeleeAttack.performed -= MeleeAttackAction;
        _inputAction.Player.RangeAttack.performed -= RangeAttackAction;
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

    private void InteractAction(InputAction.CallbackContext value)
    {
        _animator.SetTrigger(_interactTriggerName);
    }

    private void JumpAction(InputAction.CallbackContext value)
    {
        Jump();
    }

    private void AreaAttackAction(InputAction.CallbackContext value)
    {
        _animator.SetTrigger(_aAttackTriggerName);
    }

    private void MeleeAttackAction(InputAction.CallbackContext value)
    {
        _animator.SetTrigger(_mAttackTriggerName);
    }

    private void RangeAttackAction(InputAction.CallbackContext value)
    {
        _animator.SetTrigger(_rAttackTriggerName);
    }

    private void SuicideAction(InputAction.CallbackContext value)
    {
        OnDeath();
    }
    #endregion

    private void Update()
    {
        if (!_isAlive) return;

        _smoothInput = Vector2.SmoothDamp(_smoothInput, _rawInput, ref _smoothVelocity, _smoothInputSpeed);

        _groundRayOffset = new Vector3(transform.position.x, transform.position.y + _groundRayLength / 4.0f, transform.position.z);

        _groundRay = new Ray(_groundRayOffset, Vector3.down);
        _isOnAir = !Physics.Raycast(_groundRay, _groundRayLength, _groundRayMask);

        _animator.SetFloat(_xAxisName, _smoothInput.x);
        _animator.SetFloat(_yAxisName, _smoothInput.y);
        _animator.SetBool(_airBoolName, _isOnAir);
        _animator.SetBool(_moveBoolName, _rawInput.sqrMagnitude != 0.0f);
    }

    private void FixedUpdate()
    {
        if (!_isAlive) return;

        if (_rawInput.sqrMagnitude != 0.0f)
        {
            Movement(_rawInput);
        }
    }

    public void Interact()
    {
        _castRayOffset = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

        _castRay = new Ray(_castRayOffset, transform.forward);

        if (Physics.SphereCast(_castRay, _interactSphereRadius, out _entityHit, _interactRayLength, _interactableRayMask))
        {
            if (_entityHit.collider.TryGetComponent(out Interactable interactable))
            {
                interactable.Interact();
            }
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

    public void AreaAttack()
    {
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, _areaSphereRadius, _entityRayMask);

        foreach(Collider collider in overlappedColliders)
        {
            if(Mathf.Abs(collider.transform.position.y - transform.position.y) <= 2.0f)
            {
                if (collider.TryGetComponent(out Entity entity))
                {
                    entity.TakeDamage(_damage * 10);
                }
            }
        }
    }

    public void MeleeAttack()
    {
        _castRayOffset = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

        _castRay = new Ray(_castRayOffset, transform.forward);

        if(Physics.Raycast(_castRay, out _entityHit, _meleeRayLength, _entityRayMask))
        {
            if(_entityHit.collider.TryGetComponent(out Entity entity))
            {
                entity.TakeDamage(_damage);
            }
        }
    }

    public void RangeAttack()
    {
        _castRayOffset = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

        _castRay = new Ray(_castRayOffset, transform.forward);

        RaycastHit[] hits = Physics.SphereCastAll(_castRay, _rangeSphereRadius, _rangeRayLength, _entityRayMask);

        foreach(RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out Entity entity))
            {
                entity.TakeDamage(_damage * 2);
            }
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

    private void OnDeath()
    {
        _isAlive = false;

        _animator.SetTrigger(_deathTriggerName);
    }

    private void OnDrawGizmos()
    {
        if (_isOnAir)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawRay(_groundRay.origin, _groundRay.direction * _groundRayLength);        
    }
}
