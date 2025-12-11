using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;

    private Controller controller;
    private IInteractable currentInteractable;

    private void Awake()
    {
        controller = new Controller();
    }

    private void OnEnable()
    {
        controller.Enable();
        controller.Base.Interaction.performed += OnInteract;
    }

    private void OnDisable()
    {
        controller.Base.Interaction.performed -= OnInteract;
        controller.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var worldItem = other.GetComponent<WorldItemView>();
        if (worldItem != null)
        {
            TryPickupWorldItem(worldItem);
        }

        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            currentInteractable.ShowInteractionIndicator();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable.HideInteractionIndicator();
            currentInteractable = null;
        }
    }

    private void TryPickupWorldItem(WorldItemView worldItem)
    {
        if (!worldItem.gameObject.activeSelf) return;

        var instance = worldItem.ToItemInstance();
        bool added = inventoryController.TryAddItemToHotbarFirst(instance);

        if (added)
        {
            var itemDef = inventoryController.ItemLookup?.Invoke(instance.itemId);
            string itemName = itemDef != null ? itemDef.displayName : instance.itemId;
            string message = instance.amount > 1 
                ? $"+{instance.amount} {itemName}" 
                : $"+1 {itemName}";
            NotificationManager.Instance?.ShowSuccess(message);

            worldItem.MarkAsCollected();

            if (WorldItemPool.Instance != null)
            {
                WorldItemPool.Instance.Return(worldItem);
            }
            else
            {
                worldItem.gameObject.SetActive(false);
                Destroy(worldItem.gameObject);
            }
        }
        else
        {
            NotificationManager.Instance?.ShowError("Inventory full!");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentInteractable != null)
        {
            currentInteractable.Interact(this);
        }
    }
}
