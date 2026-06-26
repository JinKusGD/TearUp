using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Jump,
    Rise,
    Fall,
    Land,
    Attack
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : EntityBase, IAttackable, ITakeDamageable
{
    [Header("Components")]
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private Transform _cameraTransform;

    [Header("State")]
    [SerializeField] private PlayerState _playerState;

    [Header("WalkSettings")]
    [SerializeField] private Vector2 _playerInput;
    [SerializeField] private float _walkSpeed = 10.0f;
    [SerializeField] private float _turnSpeed = 30.0f;

    [Header("JumpSettings")]
    [SerializeField] private GroundCheck _groundCheck;
    [SerializeField] private bool _isJumpPressed;
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private bool _isGrounded;
   
    [Header("RunSettings")]
    [SerializeField] private bool _isRunPressed;
    [SerializeField] private float _runSpeed = 20.0f;

    [Header("AttackSettings")]
    [SerializeField] private bool _isAttackPressed;

    private PlayerAnimationController _playerAnimationController; 
 

    private void Awake()
    {
        if (_playerRigidbody == null)
        {
            if (!TryGetComponent(out _playerRigidbody))
            {
                Debug.LogError($"플레이어 리지드바디가 없습니다.");
                return;
            }
        }

        if (_cameraTransform == null)
        {
            if(Camera.main == null)
            {
                Debug.LogError($"참조할 메인 카메라가 없습니다.");
                return;
            }

            _cameraTransform = Camera.main.transform;
        }

        if (_groundCheck == null)
        {
            _groundCheck = GetComponentInChildren<GroundCheck>();

            if (_groundCheck == null)
            {
                Debug.LogError($"플레이어 GroundCheck 스크립트가 없습니다.");
            }
        }

        if (!TryGetComponent(out Animator animator))
        {
            Debug.LogError($"플레이어 애니메이터가 없습니다.");
            return;
        }

        _playerAnimationController = new PlayerAnimationController(animator);

        Hp = 90;
    }

    private void OnEnable()
    {
        InputManager.Instance.BindPlayerMoveAction(OnMove);
        InputManager.Instance.BindPlayerJumpAction(OnJump);
        InputManager.Instance.BindPlayerRunAction(OnRun);
        InputManager.Instance.BindPlayerAttackAction(OnAttack);
        _groundCheck.BindGroundCheckAction(OnGroundCheck);
    }

    private void Update()
    {
        if(_playerState == PlayerState.Attack) { return; }

        if (_isAttackPressed)
        {
            Attack();
            return;
        }

        if (_isGrounded)
        {
            UpdateGroundState();
        }
        else
        {
            UpdateAirState();
        }

        CurrentStateOnUpdate();
    }


    private void FixedUpdate()
    {
        AttackCooldown();

        if (_playerState == PlayerState.Attack) { return; }

        Move();

        if (_isJumpPressed)
        {
            Jump();
        }
    }

    private void AttackCooldown()
    {
        if(attackCoolDown == 0) { return; }

        float targetCoolDown = attackCoolDown -= Time.deltaTime;

        attackCoolDown = Mathf.Clamp(attackCoolDown, 0, attackCoolTime);
    }

    private void OnDisable()
    {
        InputManager.Instance.UnBindPlayerMoveAction(OnMove);
        InputManager.Instance.UnBindPlayerJumpAction(OnJump);
        InputManager.Instance.UnBindPlayerRunAction(OnRun);
        InputManager.Instance.UnBindPlayerAttackAction(OnAttack);
        _groundCheck.UnBindGroundCheckAction();
    }

    private void UpdateGroundState()
    {
        if (_playerInput == Vector2.zero)
        {
            _playerState = PlayerState.Idle;
        }
        else
        {
            if (_isRunPressed)
            {
                _playerState = PlayerState.Run;
            }
            else
            {
                _playerState = PlayerState.Walk;
            }
        }
    }

    private void UpdateAirState()
    {
        float yVelocity = _playerRigidbody.linearVelocity.y;

        if (yVelocity >= 0.1f)
        {
            _playerState = PlayerState.Rise;
        }
        else if (yVelocity <= -0.1f)
        {
            _playerState = PlayerState.Fall;
        }
    }

    private void CurrentStateOnUpdate()
    {
        _playerAnimationController.SetState(_playerState);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _playerInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        float jumpInput = context.ReadValue<float>();

        _isJumpPressed = (jumpInput != 0);
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        float RunInput = context.ReadValue<float>();

        _isRunPressed = (RunInput != 0);
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        float AttackInput = context.ReadValue<float>();

        _isAttackPressed = (AttackInput != 0);
    }

    private void OnGroundCheck(bool isGrounded)
    {
        if (isGrounded && _playerState == PlayerState.Fall)
        {
            _playerAnimationController.SetState(PlayerState.Land);
        }
        
        _isGrounded = isGrounded;
    }

    private void Move()
    {
        if(_playerInput == Vector2.zero)
        {
            return;
        }

        float moveSpeed = (_isRunPressed) ? _runSpeed : _walkSpeed;

        Vector3 inputDirection = new Vector3(_playerInput.x, 0f, _playerInput.y).normalized;

        if (inputDirection.sqrMagnitude > 0.5f)
        {
            Vector3 cameraForwardDirection = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
            Vector3 cameraRightDirection = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;

            Vector3 targetDirection = (cameraForwardDirection * inputDirection.z + cameraRightDirection * inputDirection.x).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Vector3 targetPosition = moveSpeed * Time.fixedDeltaTime * targetDirection;

            _playerRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime));
            _playerRigidbody.MovePosition(_playerRigidbody.position + targetPosition);
        }
    }

    private void Jump()
    {
        if (!_isGrounded)
        {
            return;
        }

        _isGrounded = false;

        Vector3 velocity = _playerRigidbody.linearVelocity;
        velocity.y = 0f;
        _playerRigidbody.linearVelocity = velocity;

        _playerAnimationController.SetState(PlayerState.Jump);
        _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void Attack()
    {
        if(attackCoolDown != 0) { return; }

        attackCoolDown = attackCoolTime;

        _playerState = PlayerState.Attack;
        _playerAnimationController.SetState(PlayerState.Attack);
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);

        _playerState = PlayerState.Idle;
    }

    public void OnAttackHit()
    {
        Vector3 finalCheckPosition = Chest.position + (Chest.forward * AttackOffset);

        if (hitColliders == null)
        {
            hitColliders = new Collider[maxHitCount];
        }

        int hitCount = Physics.OverlapBoxNonAlloc(finalCheckPosition, boxHalfExtents, hitColliders, Chest.rotation, targetLayer);

        for (int i = 0; i < hitCount; i++)
        {
           Collider hitCollider = hitColliders[i];

            if (hitCollider.TryGetComponent<IInstanceable>(out var enemy))
            {
                ObjectManager.Instance.RequestHitDamageByInstanceId(enemy.InstanceId, attack);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        Hp -= damage;
        Debug.Log("사운드 재생");
        Debug.Log(Hp);
    }

    public void Heal(float value)
    {
        float targetHp = Hp + value;
        Hp = Mathf.Clamp(targetHp, 0, MaxHp);
        Debug.Log("사운드 재생");
        Debug.Log(Hp);
    }

    public float MaxHp { get; private set; } = 100;

    public float Hp { get; private set; }

    [SerializeField] private Transform Chest;

    Collider[] hitColliders;
    [SerializeField] private int maxHitCount = 10; 

    [SerializeField] private float AttackOffset = 0.5f;
    [SerializeField] private LayerMask targetLayer;

    public Vector3 boxHalfExtents = new Vector3(0.3f, 0.3f, 0.25f);
    public float attack = 10.0f;
    public float attackCoolTime = 5.0f;
    [SerializeField] public float attackCoolDown = 0.0f;
}