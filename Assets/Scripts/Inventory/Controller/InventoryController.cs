using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour, InventoryView.IInventoryViewListener
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private InventoryView inventoryView;
    [SerializeField] private ItemTooltipView tooltipView;
    [SerializeField] private int inventorySize = 20;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private PlayerInventoryController playerInventoryController;
    [SerializeField] private HotbarController hotbarController;

    private Controller controller;
    private InventoryModel inventoryModel;
    private bool isOpen;

    private int? draggingFromIndex;
    private Vector3 lastPointerPosition;

    public InventoryModel Model => inventoryModel;
    public System.Func<string, ItemDefinition> ItemLookup => itemDatabase.GetItem;
    public bool IsOpen => isOpen;
    public HotbarController Hotbar => hotbarController;

    void Awake()
    {
        controller = new Controller();
        inventoryModel = new InventoryModel(inventorySize);
        inventoryModel.OnInventoryChanged += HandleInventoryChanged;
    }

    void OnEnable()
    {
        controller.Enable();
        controller.Base.OpenInventory.performed += OnToggleInventory;
    }

    void OnDisable()
    {
        controller.Base.OpenInventory.performed -= OnToggleInventory;
        controller.Disable();
    }

    void OnDestroy()
    {
        if (inventoryModel != null)
        {
            inventoryModel.OnInventoryChanged -= HandleInventoryChanged;
        }
    }

    void Start()
    {
        inventoryView.Initialize(inventorySize, this);
        HandleInventoryChanged();
        tooltipView.Hide();
        CloseInventory();
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (isOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
        tooltipView.Hide();
    }

    public void ToggleInventory()
    {
        if (isOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    void HandleInventoryChanged()
    {
        inventoryView.Refresh(inventoryModel.Slots, itemDatabase.GetItem);
    }

    public void OnSlotHoverEnter(int slotIndex, PointerEventData eventData)
    {
        var item = inventoryModel.GetItem(slotIndex);
        if (item == null)
        {
            tooltipView.Hide();
            return;
        }

        var def = itemDatabase.GetItem(item.itemId);
        if (def != null)
        {
            tooltipView.Show(def, eventData.position);
        }
        else
        {
            tooltipView.Hide();
        }
    }

    public void OnSlotHoverExit(int slotIndex, PointerEventData eventData)
    {
        tooltipView.Hide();
    }

    public void OnSlotRightClicked(int slotIndex, PointerEventData eventData)
    {
        tooltipView.Hide();
        
        var item = inventoryModel.GetItem(slotIndex);
        if (item == null) return;

        if (playerInventoryController != null)
        {
            playerInventoryController.DropFromSlot(slotIndex, 1);
        }
    }

    public void OnBeginDragSlot(int slotIndex, PointerEventData eventData)
    {
        draggingFromIndex = slotIndex;
        lastPointerPosition = eventData.position;
        tooltipView.Hide();
    }

    public void OnDragSlot(int slotIndex, PointerEventData eventData)
    {
        lastPointerPosition = eventData.position;
    }

    public void OnEndDragSlot(int slotIndex, PointerEventData eventData)
    {
        if (draggingFromIndex.HasValue)
        {
            int fromIndex = draggingFromIndex.Value;

            var raycastResults = eventData.pointerCurrentRaycast;

            var inventorySlotView = raycastResults.gameObject != null
                ? raycastResults.gameObject.GetComponentInParent<InventorySlotView>()
                : null;

            if (inventorySlotView != null)
            {
                int toIndex = inventorySlotView.SlotIndex;
                inventoryModel.Move(fromIndex, toIndex);
            }
            else
            {
                var hotbarSlotView = raycastResults.gameObject != null
                    ? raycastResults.gameObject.GetComponentInParent<HotbarSlotView>()
                    : null;

                if (hotbarSlotView != null && hotbarController != null)
                {
                    int hotbarIndex = hotbarSlotView.SlotIndex;
                    MoveToHotbar(fromIndex, hotbarIndex);
                }
            }

            draggingFromIndex = null;
        }
    }

    public bool TryAddItem(ItemInstance item)
    {
        return inventoryModel.AddItem(item, itemDatabase.GetItem);
    }

    public bool TryAddItemToHotbarFirst(ItemInstance item)
    {
        if (hotbarController != null && hotbarController.TryAddItem(item))
        {
            return true;
        }
        return TryAddItem(item);
    }

    public ItemInstance RemoveItemFromSlot(int slotIndex, int amount)
    {
        return inventoryModel.RemoveAt(slotIndex, amount);
    }

    public void MoveToHotbar(int inventorySlotIndex, int hotbarSlotIndex)
    {
        if (hotbarController == null) return;

        var item = inventoryModel.GetItem(inventorySlotIndex);
        if (item == null) return;

        var removedItem = inventoryModel.RemoveAt(inventorySlotIndex, item.amount);
        if (removedItem != null)
        {
            var existingHotbarItem = hotbarController.GetItemAtSlot(hotbarSlotIndex);
            
            if (existingHotbarItem != null)
            {
                hotbarController.Model.ClearSlot(hotbarSlotIndex);
                inventoryModel.AddItem(existingHotbarItem, itemDatabase.GetItem);
            }

            hotbarController.AssignItemToSlot(hotbarSlotIndex, removedItem);
        }
    }

    public void MoveFromHotbar(int hotbarSlotIndex, int inventorySlotIndex)
    {
        if (hotbarController == null) return;

        var item = hotbarController.GetItemAtSlot(hotbarSlotIndex);
        if (item == null) return;

        var removedItem = hotbarController.RemoveItemFromSlot(hotbarSlotIndex, item.amount);
        if (removedItem != null)
        {
            var existingInventoryItem = inventoryModel.GetItem(inventorySlotIndex);
            
            if (existingInventoryItem != null)
            {
                inventoryModel.RemoveAt(inventorySlotIndex, existingInventoryItem.amount);
                hotbarController.AssignItemToSlot(hotbarSlotIndex, existingInventoryItem);
            }

            var slot = inventoryModel.Slots[inventorySlotIndex];
            slot.item = removedItem;
            HandleInventoryChanged();
        }
    }
}
