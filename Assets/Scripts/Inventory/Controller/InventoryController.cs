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

    private Controller controller;
    private InventoryModel inventoryModel;
    private bool isOpen;

    private int? draggingFromIndex;
    private Vector3 lastPointerPosition;

    public InventoryModel Model => inventoryModel;
    public System.Func<string, ItemDefinition> ItemLookup => itemDatabase.GetItem;
    public bool IsOpen => isOpen;

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
            var slotView = raycastResults.gameObject != null
                ? raycastResults.gameObject.GetComponentInParent<InventorySlotView>()
                : null;

            if (slotView != null)
            {
                int toIndex = slotView.SlotIndex;
                inventoryModel.Move(fromIndex, toIndex);
            }

            draggingFromIndex = null;
        }
    }

    public bool TryAddItem(ItemInstance item)
    {
        return inventoryModel.AddItem(item, itemDatabase.GetItem);
    }

    public ItemInstance RemoveItemFromSlot(int slotIndex, int amount)
    {
        return inventoryModel.RemoveAt(slotIndex, amount);
    }
}
