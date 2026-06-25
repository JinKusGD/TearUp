using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private Action<bool> _groundTriggerEvent;

    public void BindGroundCheckAction(Action<bool> callback)
    {
        _groundTriggerEvent = callback;
    }

    public void UnBindGroundCheckAction()
    {
        _groundTriggerEvent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            return;
        }

        _groundTriggerEvent.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            return;
        }

        _groundTriggerEvent.Invoke(false);
    }
}
