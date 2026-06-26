using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input Actions")]
    [SerializeField] private InputActionReference _playerMoveAction;
    [SerializeField] private InputActionReference _playerJumpAction;
    [SerializeField] private InputActionReference _playerRunAction;
    [SerializeField] private InputActionReference _playerAttackAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] 이미 InputManager 인스턴스가 존재하여 생성된 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        EnableAllAction();
    }

    private void OnDisable()
    {
        DisableAllAction();
    }

    public void BindPlayerMoveAction(Action<InputAction.CallbackContext> callback)
    {
        _playerMoveAction.action.performed += callback;
    }

    public void BindPlayerJumpAction(Action<InputAction.CallbackContext> callback)
    {
        _playerJumpAction.action.performed += callback;
    }

    public void BindPlayerRunAction(Action<InputAction.CallbackContext> callback)
    {
        _playerRunAction.action.performed += callback;
    }

    public void BindPlayerAttackAction(Action<InputAction.CallbackContext> callback)
    {
        _playerAttackAction.action.performed += callback;
    }

    public void UnBindPlayerMoveAction(Action<InputAction.CallbackContext> callback)
    {
        _playerMoveAction.action.performed -= callback;
    }

    public void UnBindPlayerJumpAction(Action<InputAction.CallbackContext> callback)
    {
        _playerJumpAction.action.performed -= callback;
    }

    public void UnBindPlayerRunAction(Action<InputAction.CallbackContext> callback)
    {
        _playerRunAction.action.performed -= callback;
    }

    public void UnBindPlayerAttackAction(Action<InputAction.CallbackContext> callback)
    {
        _playerAttackAction.action.performed -= callback;
    }

    public void EnablePlayerAction()
    {
        EnablePlayerMoveAction();
        EnablePlayerJumpAction();
        EnablePlayerRunAction();
        EnablePlayerAttackAction();

    }

    public void DisablePlayerAction()
    {
        DisablePlayerMoveAction();
        DisablePlayerJumpAction();
        DisablePlayerRunAction();
        DisablePlayerAttackAction();
    }

    private void EnablePlayerMoveAction()
    {
        if (_playerMoveAction.action.enabled)
        {
            Debug.Log("플레이어 걷기 액션이 이미 활성화 되어있습니다.");
            return;
        }
        
        _playerMoveAction.action.Enable();
    }

    private void EnablePlayerJumpAction()
    {
        if (_playerJumpAction.action.enabled)
        {
            Debug.Log("플레이어 점프 액션이 이미 활성화 되어있습니다.");
            return;
        }

        _playerJumpAction.action.Enable();
    }

    private void EnablePlayerRunAction()
    {
        if (_playerRunAction.action.enabled)
        {
            Debug.Log("플레이어 달리기 액션이 이미 활성화 되어있습니다.");
            return;
        }

        _playerRunAction.action.Enable();
    }

    private void EnablePlayerAttackAction()
    {
        if (_playerAttackAction.action.enabled)
        {
            Debug.Log("플레이어 공격 액션이 이미 활성화 되어있습니다.");
            return;
        }

        _playerAttackAction.action.Enable();
    }

    private void DisablePlayerMoveAction()
    {
        if (!_playerMoveAction.action.enabled)
        {
            Debug.Log("플레이어 걷기 액션이 이미 비활성화 되어있습니다.");
            return;
        }

        _playerMoveAction.action.Disable();
    }

    private void DisablePlayerJumpAction()
    {
        if (!_playerJumpAction.action.enabled)
        {
            Debug.Log("플레이어 점프 액션이 이미 비활성화 되어있습니다.");
            return;
        }

        _playerJumpAction.action.Disable();
    }

    private void DisablePlayerRunAction()
    {
        if (!_playerRunAction.action.enabled)
        {
            Debug.Log("플레이어 달리기 액션이 이미 비활성화 되어있습니다.");
            return;
        }

        _playerRunAction.action.Disable();
    }

    private void DisablePlayerAttackAction()
    {
        if (!_playerAttackAction.action.enabled)
        {
            Debug.Log("플레이어 공격 액션이 이미 비활성화 되어있습니다.");
            return;
        }

        _playerAttackAction.action.Disable();
    }

    private void EnableAllAction()
    {
        EnablePlayerAction();
    }

    private void DisableAllAction()
    {
        DisablePlayerAction();
    }
}