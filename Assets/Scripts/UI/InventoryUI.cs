using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _scrollViewContentTransform;

    private Dictionary<long, InventorySlot> _slotDict = new Dictionary<long, InventorySlot>();

    private long _selectedSlotItemUId;

    private void Start()
    {
        InitializeInventory();
        InventoryManager.Instance.BindInventoryChangeAction(OnInventoryChanged);
    }

    private void InitializeInventory()
    {
        List<ItemData> inventoryItemDataList = InventoryManager.Instance.GetInventoryItemDataList();

        foreach (ItemData itemData in inventoryItemDataList)
        {
            AddSlot(itemData);
        }
    }

    private void AddSlot(ItemData itemData)
    {
        GameObject slotObject = Instantiate(_slotPrefab, _scrollViewContentTransform);

        if (!slotObject.TryGetComponent(out InventorySlot inventorySlot))
        {
            Debug.LogError("인벤토리 슬롯이 아닌 프리팹");
            return;
        }

        inventorySlot.InitializeSlot(itemData.ItemUId, itemData.itemName, itemData.ItemCount);

        _slotDict.Add(itemData.ItemUId, inventorySlot);
        inventorySlot.BindSlotClickAction(OnSlotClick);
    }

    private void RemoveSlot(long itemUId)
    {
        Debug.Log($"{itemUId} 슬롯 제거");
    }

    private void OnInventoryChanged(ItemData itemData)
    {
        if(itemData == null)
        {
            Debug.LogError("올바르지 않은 아이템데이터입니다.");
            return;
        }

        int itemCount = itemData.ItemCount;

        if (itemCount <= 0)
        {
            RemoveSlot(itemData.ItemUId);
            return;
        }

        if (!_slotDict.TryGetValue(itemData.ItemUId, out InventorySlot inventorySlot))
        {
            AddSlot(itemData);
            return;
        }

        inventorySlot.ChangeCountText(itemCount);
    }

    private void OnSlotClick(long ItemUId)
    {
        if (!_slotDict.ContainsKey(ItemUId))
        {
            Debug.LogError("인벤토리에 해당 UId 아이템을 가지고 있지 않습니다.");
            return;
        }

        _selectedSlotItemUId = ItemUId;
    }
}