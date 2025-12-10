using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private GameObject worldItemPrefab;
    [SerializeField] private Transform dropOrigin;

    public void DropFromSlot(int slotIndex, int amount)
    {
        var itemInstance = inventoryController.RemoveItemFromSlot(slotIndex, amount);
        if (itemInstance == null)
            return;

        var go = Instantiate(worldItemPrefab, dropOrigin.position, Quaternion.identity);
        var worldItem = go.GetComponent<WorldItemView>();
        if (worldItem != null)
        {
            worldItem.SetItem(itemInstance.itemId, itemInstance.amount);
        }
    }
}
