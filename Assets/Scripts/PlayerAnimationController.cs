using UnityEngine;

public class PlayerAnimationController
{
    private readonly Animator _animator;
    private PlayerState _currentState;

    public PlayerAnimationController(Animator animator)
    {
        _animator = animator;
    }

    public void SetState(PlayerState newState)
    {
        if (newState == _currentState)
        {
            return;
        }

        _currentState = newState;

        ResetAllState();

        switch (_currentState)
        {
            case PlayerState.Walk:
                _animator.SetBool("Walk", true);
                break;
            case PlayerState.Run:
                _animator.SetBool("Run", true);
                break;
            case PlayerState.Jump:
                _animator.SetTrigger("Jump");
                break;
            case PlayerState.Fall:
                _animator.SetBool("Fall", true);
                break;
            case PlayerState.Land:
                _animator.SetTrigger("Land");
                break;
        }
    }

    private void ResetAllState()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Run", false);
        _animator.SetBool("Fall", false);
    }
}
