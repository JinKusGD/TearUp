using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class ButtonBase : MonoBehaviour
{
    [SerializeField] protected Button _button;
    [SerializeField] protected Image _buttonImage;
    [SerializeField] protected Text _buttonText;

    protected virtual void OnDisable()
    {
        UnBindOnClickEvent();
    }

    public virtual void BindOnClickEvent(UnityAction onClick)
    {
        if (_button == null) { return; }

        _button.onClick.AddListener(onClick);
    }

    protected virtual void UnBindOnClickEvent()
    {
        if (_button == null) { return; }

        _button.onClick.RemoveAllListeners();
    }
}
