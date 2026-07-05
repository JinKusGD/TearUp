using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private GameObject _inventoryUI;

    private GameObject _createdInventoryUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_createdInventoryUI == null)
            {
                _createdInventoryUI = CreateInventory();
                _createdInventoryUI.SetActive(true);
                return;
            }

            bool isActive = _createdInventoryUI.activeSelf;
            _createdInventoryUI.SetActive(!isActive);
        }
    }

    private GameObject CreateInventory()
    {
        GameObject InventoryUi = Instantiate(_inventoryUI, _canvas);

        return InventoryUi;
    }
}
