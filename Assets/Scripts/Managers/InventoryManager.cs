using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public enum UseItemType
{
    None,
    HpUp,
    SpeedUp
}

public class ItemData
{
    public long ItemUId;
    public string ItemName;
    public ItemType ItemType;
    public UseItemType UseItemType;
    public float Value;
    public int ItemCount;
    public int ItemMaxCount;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private readonly Dictionary<ItemType, int> maxCounts = new Dictionary<ItemType, int>();

    private readonly List<ItemData> _inventory = new List<ItemData>();

    private Action<ItemData> _onInventoryChanged;

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

    public void BindInventoryChangeAction(Action<ItemData> InventoryChangedCallback)
    {
        _onInventoryChanged += InventoryChangedCallback;
    }

    public List<ItemData> GetInventoryItemDataList()
    {
        return _inventory;
    }

    public void AddItem(ItemType itemType, string itemName, UseItemType useItemType, float value, int addCount)
    {
        int remainingAddCount = addCount;

        foreach (ItemData inventoryItemData in _inventory) 
        {
            if (inventoryItemData.ItemName != itemName)
            {
                continue;
            }

            if (inventoryItemData.ItemCount >= inventoryItemData.ItemMaxCount)
            {
                continue;
            }

            int RemainingCount = inventoryItemData.ItemMaxCount - inventoryItemData.ItemCount;

            int amountToAdd = Mathf.Min(RemainingCount, remainingAddCount);

            inventoryItemData.ItemCount += amountToAdd;
            remainingAddCount -= amountToAdd;
            InvokeChangeAction(inventoryItemData);

            if (remainingAddCount <= 0) { break; }
        }

        while (remainingAddCount > 0)
        {
            int addedCount = AddNewItem(itemType, itemName, useItemType, value, remainingAddCount);
            remainingAddCount -= addedCount;
        }
    }

    private int AddNewItem(ItemType itemType, string itemName, UseItemType useItemType, float value, int addCount)
    {
        long itemUId = GenerateUniqueId();
        
        ItemData newItem = new ItemData();

        newItem.ItemUId = itemUId;
        newItem.ItemName = itemName;
        newItem.ItemType = itemType;
        newItem.UseItemType = useItemType;
        newItem.Value = value;

        if (!maxCounts.ContainsKey(itemType))
        {
            Debug.LogError("아이템 타입에 대한 최대 값이 설정되지 않았습니다.");
        }

        int maxCount = maxCounts[itemType];

        addCount = Mathf.Min(addCount, maxCount);

        newItem.ItemCount = addCount;
        newItem.ItemMaxCount = maxCount;

        _inventory.Add(newItem);
        InvokeChangeAction(newItem);

        return addCount;
    }

    private void InitMaxCount()
    {
        maxCounts[ItemType.Equip] = 1;
        maxCounts[ItemType.Use] = 99;
    }

    private void InvokeChangeAction(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("올바르지 않은 아이템 데이터 입니다.");
            return;
        }

        if (_onInventoryChanged == null)
        {
            return;
        }

        _onInventoryChanged.Invoke(itemData);
    }


    public bool RequestUseItem(long useItemUId)
    {
        bool useItemSuccess;

        int removeIndex = 0;

        foreach (ItemData itemData in _inventory)
        {
            if (itemData.ItemUId != useItemUId)
            {
                removeIndex++;
                continue;
            }

            UseItem(itemData.UseItemType, itemData.Value);
            break;
        }

        useItemSuccess = RequestRemoveItem(removeIndex);
        
        return useItemSuccess;
    }

    private void UseItem(UseItemType useItemType, float value)
    {
        if (useItemType == UseItemType.None || value == 0)
        {
            return;
        }

        GameObject player = ObjectManager.Instance.GetObjectByInstanceId(1);
        
        if (player == null) 
        {
            Debug.LogError("플레이어 없음");
        }

        if(!player.TryGetComponent(out PlayerController playerController))
        {
            Debug.LogError("플레이어 컨트롤러 없음");
        }

        playerController.UseItem(useItemType, value);
    }

    private bool RequestRemoveItem(int removeIndex)
    {
        ItemData itemData = _inventory[removeIndex];

        if (itemData == null)
        {
            Debug.LogError("올바르지 않은 아이템 정보");
            return false;
        }

        itemData.ItemCount--;

        if (itemData.ItemCount <= 0)
        {
            _inventory.RemoveAt(removeIndex);
        }

        InvokeChangeAction(itemData);

        return true;
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
