using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private InventorySlotView slotPrefab;
    [SerializeField] private Transform slotsParent;

    private List<InventorySlotView> slotViews = new();
    private IInventoryViewListener listener;

    public interface IInventoryViewListener
    {
        void OnSlotRightClicked(int slotIndex, PointerEventData eventData);
        void OnSlotHoverEnter(int slotIndex, PointerEventData eventData);
        void OnSlotHoverExit(int slotIndex, PointerEventData eventData);
        void OnBeginDragSlot(int slotIndex, PointerEventData eventData);
        void OnDragSlot(int slotIndex, PointerEventData eventData);
        void OnEndDragSlot(int slotIndex, PointerEventData eventData);
    }

    public void Initialize(int slotCount, IInventoryViewListener listener)
    {
        this.listener = listener;

        for (int i = 0; i < slotCount; i++)
        {
            var slot = Instantiate(slotPrefab, slotsParent);
            slot.Initialize(i, this);
            slotViews.Add(slot);
        }
    }

    public void Refresh(IReadOnlyList<InventorySlot> slots, System.Func<string, ItemDefinition> itemLookup)
    {
        for (int i = 0; i < slotViews.Count; i++)
        {
            var view = slotViews[i];
            if (i >= slots.Count)
            {
                view.SetEmpty();
                continue;
            }

            var slot = slots[i];
            if (slot.IsEmpty)
            {
                view.SetEmpty();
            }
            else
            {
                var def = itemLookup(slot.item.itemId);
                if (def != null)
                {
                    view.SetItem(def.icon, slot.item.amount, def.stackable);
                }
                else
                {
                    view.SetEmpty();
                }
            }
        }
    }

    public void NotifySlotRightClicked(int slotIndex, PointerEventData eventData)
    {
        listener?.OnSlotRightClicked(slotIndex, eventData);
    }

    public void NotifySlotHoverEnter(int slotIndex, PointerEventData eventData)
    {
        listener?.OnSlotHoverEnter(slotIndex, eventData);
    }

    public void NotifySlotHoverExit(int slotIndex, PointerEventData eventData)
    {
        listener?.OnSlotHoverExit(slotIndex, eventData);
    }

    public void NotifyBeginDrag(int slotIndex, PointerEventData eventData)
    {
        listener?.OnBeginDragSlot(slotIndex, eventData);
    }

    public void NotifyDrag(int slotIndex, PointerEventData eventData)
    {
        listener?.OnDragSlot(slotIndex, eventData);
    }

    public void NotifyEndDrag(int slotIndex, PointerEventData eventData)
    {
        listener?.OnEndDragSlot(slotIndex, eventData);
    }
}
