using System;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _selectImage;
    [SerializeField] private InventorySlotButton _inventorySlotButton;

    private Action<long> _onSlotClickAction;
    private long _slotItemUId;

    private void OnEnable()
    {
        _inventorySlotButton.BindOnClickEvent(OnSlotClick);
    }

    private void OnDestroy()
    {
        UnBindSlotClickAction();
    }

    public void InitializeSlot(long itemUId, string itemName, int itemCount)
    {
        _slotItemUId = itemUId;

        ChangeIcon(itemName);
        ChangeCountText(itemCount);
    }

    public void ChangeCountText(int count)
    {
        _inventorySlotButton.ChangeCount(count);
    }

    public void BindSlotClickAction(Action<long> slotClickAction)
    {
        _onSlotClickAction = slotClickAction;
    }

    private void UnBindSlotClickAction()
    {
        if (_onSlotClickAction == null) 
        {
            return;
        }

        _onSlotClickAction = null;
    }

    private void ChangeIcon(string itemName)
    {
        string resourcePath = $"ItemIcons/{itemName}";

        Sprite sprite = Resources.Load<Sprite>(resourcePath);

        if (sprite == null)
        {
            Debug.LogError($"{resourcePath} 에셋을 찾을 수 없습니다.");
            return;
        }

        _inventorySlotButton.ChangeIcon(sprite);
    }

    private void OnSlotClick()
    {
        if (_onSlotClickAction == null)
        {
            return;
        }

        _onSlotClickAction.Invoke(_slotItemUId);
    }

    public void ChangeSelectImage(Color color)
    {
        _selectImage.color = color;
    }
}
