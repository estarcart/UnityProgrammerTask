using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class HotbarController : MonoBehaviour, HotbarView.IHotbarViewListener
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private HotbarView hotbarView;
    [SerializeField] private ItemTooltipView tooltipView;
    [SerializeField] private int hotbarSize = 9;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private PlayerInventoryController playerInventoryController;

    private Controller controller;
    private HotbarModel hotbarModel;
    private int? draggingFromIndex;

    public HotbarModel Model => hotbarModel;
    public System.Func<string, ItemDefinition> ItemLookup => itemDatabase.GetItem;

    public event Action<int, ItemInstance, ItemDefinition> OnUseItem;
    public event Action<int> OnActiveSlotChanged;

    void Awake()
    {
        controller = new Controller();
        hotbarModel = new HotbarModel(hotbarSize);
        hotbarModel.OnHotbarChanged += HandleHotbarChanged;
        hotbarModel.OnActiveSlotChanged += HandleActiveSlotChanged;
    }

    void OnEnable()
    {
        controller.Enable();
        SubscribeToHotbarInputs();
    }

    void OnDisable()
    {
        UnsubscribeFromHotbarInputs();
        controller.Disable();
    }

    void OnDestroy()
    {
        if (hotbarModel != null)
        {
            hotbarModel.OnHotbarChanged -= HandleHotbarChanged;
            hotbarModel.OnActiveSlotChanged -= HandleActiveSlotChanged;
        }
    }

    void Start()
    {
        hotbarView.Initialize(hotbarSize, this);
        HandleHotbarChanged();
        HandleActiveSlotChanged(hotbarModel.ActiveSlotIndex);
    }

    private void SubscribeToHotbarInputs()
    {
        controller.Base.HotbarSlot1.performed += ctx => SelectSlot(0);
        controller.Base.HotbarSlot2.performed += ctx => SelectSlot(1);
        controller.Base.HotbarSlot3.performed += ctx => SelectSlot(2);
        controller.Base.HotbarSlot4.performed += ctx => SelectSlot(3);
        controller.Base.HotbarSlot5.performed += ctx => SelectSlot(4);
        controller.Base.HotbarSlot6.performed += ctx => SelectSlot(5);
        controller.Base.HotbarSlot7.performed += ctx => SelectSlot(6);
        controller.Base.HotbarSlot8.performed += ctx => SelectSlot(7);
        controller.Base.HotbarSlot9.performed += ctx => SelectSlot(8);
        controller.Base.UseItem.performed += OnUseItemInput;
        controller.Base.DropItem.performed += OnDropItemInput;
    }

    private void UnsubscribeFromHotbarInputs()
    {
        controller.Base.UseItem.performed -= OnUseItemInput;
        controller.Base.DropItem.performed -= OnDropItemInput;
    }

    private void SelectSlot(int index)
    {
        hotbarModel.SetActiveSlot(index);
    }

    private void OnUseItemInput(InputAction.CallbackContext context)
    {
        UseActiveItem();
    }

    private void OnDropItemInput(InputAction.CallbackContext context)
    {
        DropActiveItem();
    }

    public void DropActiveItem()
    {
        int activeIndex = hotbarModel.ActiveSlotIndex;
        var item = hotbarModel.GetActiveItem();
        if (item == null) return;

        playerInventoryController.DropFromHotbar(activeIndex, 1);
    }

    public void UseActiveItem()
    {
        var item = hotbarModel.GetActiveItem();
        if (item == null) return;

        var def = itemDatabase.GetItem(item.itemId);
        if (def == null) return;

        OnUseItem?.Invoke(hotbarModel.ActiveSlotIndex, item, def);
    }

    private void HandleHotbarChanged()
    {
        hotbarView.Refresh(hotbarModel.Slots, itemDatabase.GetItem);
    }

    private void HandleActiveSlotChanged(int slotIndex)
    {
        hotbarView.SetActiveSlot(slotIndex);
        OnActiveSlotChanged?.Invoke(slotIndex);
    }

    public bool TryAddItem(ItemInstance item)
    {
        return hotbarModel.AddItem(item, itemDatabase.GetItem);
    }

    public ItemInstance RemoveItemFromSlot(int slotIndex, int amount)
    {
        return hotbarModel.RemoveAt(slotIndex, amount);
    }

    public void AssignItemToSlot(int slotIndex, ItemInstance item)
    {
        hotbarModel.SetItem(slotIndex, item);
    }

    public ItemInstance GetItemAtSlot(int slotIndex)
    {
        return hotbarModel.GetItem(slotIndex);
    }

    public int GetActiveSlotIndex()
    {
        return hotbarModel.ActiveSlotIndex;
    }

    public ItemInstance GetActiveItem()
    {
        return hotbarModel.GetActiveItem();
    }

    #region IHotbarViewListener Implementation

    public void OnHotbarSlotClicked(int slotIndex, PointerEventData eventData)
    {
        hotbarModel.SetActiveSlot(slotIndex);
    }

    public void OnHotbarSlotRightClicked(int slotIndex, PointerEventData eventData)
    {
        tooltipView?.Hide();

        var item = hotbarModel.GetItem(slotIndex);
        if (item == null) return;

        if (playerInventoryController != null)
        {
            playerInventoryController.DropFromHotbar(slotIndex, 1);
        }
    }

    public void OnHotbarSlotHoverEnter(int slotIndex, PointerEventData eventData)
    {
        var item = hotbarModel.GetItem(slotIndex);
        if (item == null)
        {
            tooltipView?.Hide();
            return;
        }

        var def = itemDatabase.GetItem(item.itemId);
        if (def != null)
        {
            tooltipView?.Show(def, eventData.position);
        }
        else
        {
            tooltipView?.Hide();
        }
    }

    public void OnHotbarSlotHoverExit(int slotIndex, PointerEventData eventData)
    {
        tooltipView?.Hide();
    }

    public void OnHotbarBeginDrag(int slotIndex, PointerEventData eventData)
    {
        draggingFromIndex = slotIndex;
        tooltipView?.Hide();
    }

    public void OnHotbarDrag(int slotIndex, PointerEventData eventData)
    {
        // Drag visual feedback can be implemented here
    }

    public void OnHotbarEndDrag(int slotIndex, PointerEventData eventData)
    {
        if (draggingFromIndex.HasValue)
        {
            int fromIndex = draggingFromIndex.Value;

            var raycastResults = eventData.pointerCurrentRaycast;
            
            // Check if dropped on hotbar slot
            var hotbarSlotView = raycastResults.gameObject != null
                ? raycastResults.gameObject.GetComponentInParent<HotbarSlotView>()
                : null;

            if (hotbarSlotView != null)
            {
                int toIndex = hotbarSlotView.SlotIndex;
                hotbarModel.SwapSlots(fromIndex, toIndex);
            }
            else
            {
                // Check if dropped on inventory slot
                var inventorySlotView = raycastResults.gameObject != null
                    ? raycastResults.gameObject.GetComponentInParent<InventorySlotView>()
                    : null;

                if (inventorySlotView != null && inventoryController != null)
                {
                    int inventoryIndex = inventorySlotView.SlotIndex;
                    inventoryController.MoveFromHotbar(fromIndex, inventoryIndex);
                }
            }

            draggingFromIndex = null;
        }
    }

    #endregion
}
