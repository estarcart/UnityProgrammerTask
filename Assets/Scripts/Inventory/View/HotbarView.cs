using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarView : MonoBehaviour
{
    [SerializeField] private HotbarSlotView slotPrefab;
    [SerializeField] private Transform slotsParent;

    private List<HotbarSlotView> slotViews = new();
    private IHotbarViewListener listener;

    public interface IHotbarViewListener
    {
        void OnHotbarSlotClicked(int slotIndex, PointerEventData eventData);
        void OnHotbarSlotRightClicked(int slotIndex, PointerEventData eventData);
        void OnHotbarSlotHoverEnter(int slotIndex, PointerEventData eventData);
        void OnHotbarSlotHoverExit(int slotIndex, PointerEventData eventData);
        void OnHotbarBeginDrag(int slotIndex, PointerEventData eventData);
        void OnHotbarDrag(int slotIndex, PointerEventData eventData);
        void OnHotbarEndDrag(int slotIndex, PointerEventData eventData);
    }

    public void Initialize(int slotCount, IHotbarViewListener listener)
    {
        this.listener = listener;

        for (int i = 0; i < slotCount; i++)
        {
            var slot = Instantiate(slotPrefab, slotsParent);
            string hotkeyLabel = (i + 1).ToString();
            if (i == 9) hotkeyLabel = "0";
            
            slot.Initialize(i, this, hotkeyLabel);
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

    public void SetActiveSlot(int slotIndex)
    {
        for (int i = 0; i < slotViews.Count; i++)
        {
            slotViews[i].SetSelected(i == slotIndex);
        }
    }

    public HotbarSlotView GetSlotView(int index)
    {
        if (index < 0 || index >= slotViews.Count)
            return null;

        return slotViews[index];
    }

    public void NotifySlotClicked(int slotIndex, PointerEventData eventData)
    {
        listener?.OnHotbarSlotClicked(slotIndex, eventData);
    }

    public void NotifySlotRightClicked(int slotIndex, PointerEventData eventData)
    {
        listener?.OnHotbarSlotRightClicked(slotIndex, eventData);
    }

    public void NotifySlotHoverEnter(int slotIndex, PointerEventData eventData)
    {
        listener?.OnHotbarSlotHoverEnter(slotIndex, eventData);
    }

    public void NotifySlotHoverExit(int slotIndex, PointerEventData eventData)
    {
        listener?.OnHotbarSlotHoverExit(slotIndex, eventData);
    }

    public void NotifyBeginDrag(int slotIndex, PointerEventData eventData)
    {
        listener?.OnHotbarBeginDrag(slotIndex, eventData);
    }

    public void NotifyDrag(int slotIndex, PointerEventData eventData)
    {
        listener?.OnHotbarDrag(slotIndex, eventData);
    }

    public void NotifyEndDrag(int slotIndex, PointerEventData eventData)
    {
        listener?.OnHotbarEndDrag(slotIndex, eventData);
    }
}
