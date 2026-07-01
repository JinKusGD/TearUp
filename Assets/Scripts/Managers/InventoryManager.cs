using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ItemData
{
    public long ItemUId;
    public string itemName;
    public int ItemCount;
    public int ItemMaxCount;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private readonly Dictionary<ItemType, int> maxCounts = new Dictionary<ItemType, int>();

    private readonly List<ItemData> _inventory = new List<ItemData>();

    private Action<string> _onInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] 이미 InventoryManager 인스턴스가 존재하여 생성된 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitMaxCount();
    }

    public void BindInventoryChangeAction(Action<string> InventoryChangedCallback)
    {
        _onInventoryChanged += InventoryChangedCallback;
    }

    public List<ItemData> GetInventoryItemDataList()
    {
        return _inventory;
    }

    public void AddItem(ItemType itemType, string itemName, int addCount)
    {
        int remainingAddCount = addCount;

        foreach (ItemData inventoryItemData in _inventory) 
        {
            if (inventoryItemData.itemName != itemName)
            {
                continue;
            }

            if (inventoryItemData.ItemCount >= inventoryItemData.ItemMaxCount)
            {
                continue;
            }

            int RemainingCount = inventoryItemData.ItemMaxCount - inventoryItemData.ItemCount;

            if (RemainingCount >= remainingAddCount)
            {
                inventoryItemData.ItemCount += remainingAddCount;
                remainingAddCount = 0;
                break;
            }

            inventoryItemData.ItemCount += RemainingCount;
            remainingAddCount -= RemainingCount;
        }

        while (remainingAddCount > 0)
        {
            int addedCount = AddNewItem(itemType, itemName, remainingAddCount);
            remainingAddCount -= addedCount;
        }
    }

    private int AddNewItem(ItemType itemType, string itemName, int addCount)
    {
        long itemUId = GenerateUniqueId();
        
        ItemData newItem = new ItemData();
        newItem.ItemUId = itemUId;
        newItem.itemName = itemName;

        if (!maxCounts.ContainsKey(itemType))
        {
            Debug.LogError("아이템 타입에 대한 최대 값이 설정되지 않았습니다.");
        }

        int maxCount = maxCounts[itemType];

        addCount = Mathf.Min(addCount, maxCount);

        newItem.ItemCount = addCount;
        newItem.ItemMaxCount = maxCount;

        _inventory.Add(newItem);

        return addCount;
    }

    private void InitMaxCount()
    {
        maxCounts[ItemType.Equip] = 1;
        maxCounts[ItemType.Use] = 999;
    }

    private static long _lastId = 0;

    public long GenerateUniqueId()
    {
        long newId = DateTime.UtcNow.Ticks;

        while (true)
        {
            long lastId = Volatile.Read(ref _lastId);

            long idToAssign = (newId <= lastId) ? lastId + 1 : newId;

            if (Interlocked.CompareExchange(ref _lastId, idToAssign, lastId) == lastId)
            {
                return idToAssign;
            }
        }
    }

}
