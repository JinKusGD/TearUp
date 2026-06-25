using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Walk,
    Jump,
    Rise,
    Fall
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private Transform _cameraTransform;

    [Header("State")]
    [SerializeField] private PlayerState _playerState;

    [Header("MoveSettings")]
    [SerializeField] private Vector2 _playerInput;
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private float _turnSpeed = 30.0f;

    [Header("JumpSettings")]
    [SerializeField] private GroundCheck _groundCheck;
    [SerializeField] private bool _isJumpPressed;
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private bool _isGrounded;

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
    }

    private void OnEnable()
    {
        InputManager.Instance.BindPlayerMoveAction(OnMove);
        InputManager.Instance.BindPlayerJumpAction(OnJump);
        _groundCheck.BindGroundCheckAction(OnGroundCheck);
    }

    private void Update()
    {
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
        Move();

        if (_isJumpPressed)
        {
            Jump();
        }
    }

    private void OnDisable()
    {
        InputManager.Instance.UnBindPlayerMoveAction(OnMove);
        InputManager.Instance.UnBindPlayerJumpAction(OnJump);
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
            _playerState = PlayerState.Walk;
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

        switch (_playerState)
        {
            case PlayerState.Idle:
                IdleState();
                break;
            case PlayerState.Walk:
                WalkState();
                break;
            case PlayerState.Jump:
                JumpState();
                break;
            case PlayerState.Rise:
                RiseState();
                break;
            case PlayerState.Fall:
                FallState();
                break;
        }
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
    private void OnGroundCheck(bool isGrounded)
    {
        _isGrounded = isGrounded;
    }

    private void Move()
    {
        if(_playerInput == Vector2.zero)
        {
            return;
        }

        Vector3 inputDirection = new Vector3(_playerInput.x, 0f, _playerInput.y).normalized;

        if (inputDirection.sqrMagnitude > 0.5f)
        {
            Vector3 cameraForwardDirection = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
            Vector3 cameraRightDirection = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;

            Vector3 targetDirection = (cameraForwardDirection * inputDirection.z + cameraRightDirection * inputDirection.x).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Vector3 targetPosition = _moveSpeed * Time.fixedDeltaTime * targetDirection;

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

    private void IdleState()
    {
        Debug.Log("Idle 상태");
    }

    private void WalkState()
    {
        Debug.Log("Move 상태");
    }

    private void JumpState()
    {
        Debug.Log("jump 상태");
    }

    private void RiseState()
    {
        Debug.Log("Rise 상태");
        //_playerRigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime;

        // 예: _animator.SetBool("IsFalling", true);
    }

    private void FallState()
    {
        Debug.Log("Fall 상태");
        //_playerRigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime;

        // 예: _animator.SetBool("IsFalling", true);
    }
}