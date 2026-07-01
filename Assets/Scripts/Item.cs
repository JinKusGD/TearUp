using UnityEngine;

public enum ItemType
{
    Equip,
    Use
}

[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
    [SerializeField] private ItemType _itemType;
    [SerializeField] private string _itemName;
    [SerializeField] private int _addCount;

    private void Awake()
    {
        Collider collider = GetComponent<Collider>();

        if (collider == null)
        {
            Debug.LogError("아이템이 콜라이더가 없습니다.");
            return;
        }

        if (collider.isTrigger)
        {
            return;
        }
        
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        InventoryManager.Instance.AddItem(_itemType, _itemName, _addCount);

        gameObject.SetActive(false);
    }
}
