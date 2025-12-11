using UnityEngine;

public class ItemUseController : MonoBehaviour
{
    [SerializeField] private HotbarController hotbarController;
    [SerializeField] private GameObject itemUser;

    void OnEnable()
    {
        if (hotbarController != null)
        {
            hotbarController.OnUseItem += HandleUseItem;
        }
    }

    void OnDisable()
    {
        if (hotbarController != null)
        {
            hotbarController.OnUseItem -= HandleUseItem;
        }
    }

    private void HandleUseItem(int slotIndex, ItemInstance itemInstance, ItemDefinition itemDefinition)
    {
        if (itemDefinition == null) return;
        if (!itemDefinition.isUsable) return;
        if (itemDefinition.useEffect == null) return;

        GameObject user = itemUser != null ? itemUser : gameObject;
        bool effectApplied = itemDefinition.useEffect.Apply(user, itemInstance, itemDefinition);

        if (effectApplied)
        {
            hotbarController.Model.ConsumeActiveItem(1);
        }
    }
}
