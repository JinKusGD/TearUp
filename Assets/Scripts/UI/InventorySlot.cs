using System;
using System.Collections;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private InventorySlotButton _inventorySlotButton;

    private Action<long> _onSlotClickAction;
    private long _slotItemUId;

    private void OnEnable()
    {
        _inventorySlotButton.BindOnClickEvent(OnSlotClick);
    }

    private void OnDisable()
    {
        UnBindSlotClickAction();
    }

    public void InitializeSlot(long itemUId, string itemName, int itemCount)
    {
        _slotItemUId = itemUId;

        StartCoroutine(LoadSlotIcon(itemName));
        _inventorySlotButton.ChangeCount(itemCount);
    }

    private IEnumerator LoadSlotIcon(string itemName)
    {
        string resourcePath = $"ItemIcons/{itemName}";

        ResourceRequest resourceRequest = Resources.LoadAsync<Sprite>(resourcePath);

        yield return resourceRequest;

        if (resourceRequest.asset == null)
        {
            Debug.LogError($"[InitSlot] 에셋을 찾을 수 없습니다.");
            yield break;
        }

        Sprite loadedSprite = resourceRequest.asset as Sprite;

        _inventorySlotButton.ChangeIcon(loadedSprite);
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

    private void OnSlotClick()
    {
        if (_onSlotClickAction == null)
        {
            return;
        }

        _onSlotClickAction.Invoke(_slotItemUId);
    }
}
