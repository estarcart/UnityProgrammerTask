using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;

    void OnTriggerEnter2D(Collider2D other)
    {
        var worldItem = other.GetComponent<WorldItemView>();
        if (worldItem == null) return;
        
        if (!worldItem.gameObject.activeSelf) return;

        var instance = worldItem.ToItemInstance();
        bool added = inventoryController.TryAddItem(instance);
        
        if (added)
        {
            worldItem.gameObject.SetActive(false);
            Destroy(worldItem.gameObject);
        }
    }
}
