using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private Transform _cameraTransform;

    [Header("MoveSettings")]
    [SerializeField] private Vector2 _playerInput;
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private float _turnSpeed = 30.0f;

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
    }

    private void OnEnable()
    {
        InputManager.Instance.BindPlayerMoveAction(OnMove);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnDisable()
    {
        InputManager.Instance.UnBindPlayerMoveAction(OnMove);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _playerInput = context.ReadValue<Vector2>();
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
}