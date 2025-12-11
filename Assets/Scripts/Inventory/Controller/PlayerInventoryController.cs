using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private HotbarController hotbarController;
    [SerializeField] private Transform dropOrigin;

    public void DropFromSlot(int slotIndex, int amount)
    {
        var itemInstance = inventoryController.RemoveItemFromSlot(slotIndex, amount);
        if (itemInstance == null)
            return;

        var def = inventoryController.ItemLookup?.Invoke(itemInstance.itemId);
        
        SpawnWorldItem(itemInstance, def);
    }

    public void DropFromHotbar(int slotIndex, int amount)
    {
        if (hotbarController == null) return;

        var itemInstance = hotbarController.RemoveItemFromSlot(slotIndex, amount);
        if (itemInstance == null)
            return;

        var def = hotbarController.ItemLookup?.Invoke(itemInstance.itemId);
        
        SpawnWorldItem(itemInstance, def);
    }

    private void SpawnWorldItem(ItemInstance itemInstance, ItemDefinition def)
    {
        if (WorldItemPool.Instance != null)
        {
            WorldItemPool.Instance.Get(
                dropOrigin.position,
                itemInstance.itemId,
                itemInstance.amount,
                def?.icon
            );

            string itemName = def != null ? def.displayName : itemInstance.itemId;
            string message = itemInstance.amount > 1 
                ? $"-{itemInstance.amount} {itemName}" 
                : $"-1 {itemName}";
            NotificationManager.Instance?.ShowError(message);
        }
    }
}
