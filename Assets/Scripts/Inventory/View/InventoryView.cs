using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private InventorySlotView slotPrefab;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private Image dragIcon;

    private List<InventorySlotView> slotViews = new();
    private IInventoryViewListener listener;
    private System.Func<string, ItemDefinition> itemLookup;
    private int? draggingSlotIndex;

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
        this.itemLookup = itemLookup;

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
        ShowDragIcon(slotIndex, eventData.position);
    }

    public void NotifyDrag(int slotIndex, PointerEventData eventData)
    {
        listener?.OnDragSlot(slotIndex, eventData);
        UpdateDragIconPosition(eventData.position);
    }

    public void NotifyEndDrag(int slotIndex, PointerEventData eventData)
    {
        listener?.OnEndDragSlot(slotIndex, eventData);
        HideDragIcon();
    }

    private void ShowDragIcon(int slotIndex, Vector2 position)
    {
        if (dragIcon == null || slotIndex < 0 || slotIndex >= slotViews.Count) return;

        var slotView = slotViews[slotIndex];
        var sprite = slotView.GetIcon();

        if (sprite == null)
        {
            dragIcon.gameObject.SetActive(false);
            return;
        }

        draggingSlotIndex = slotIndex;
        slotView.SetIconVisible(false);
        dragIcon.sprite = sprite;
        dragIcon.gameObject.SetActive(true);
        UpdateDragIconPosition(position);
    }

    private void UpdateDragIconPosition(Vector2 screenPosition)
    {
        if (dragIcon == null || !dragIcon.gameObject.activeSelf) return;

        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                screenPosition,
                parentCanvas.worldCamera,
                out Vector2 localPoint);
            dragIcon.rectTransform.localPosition = localPoint;
        }
        else
        {
            dragIcon.rectTransform.position = screenPosition;
        }
    }

    private void HideDragIcon()
    {
        if (draggingSlotIndex.HasValue && draggingSlotIndex.Value < slotViews.Count)
        {
            slotViews[draggingSlotIndex.Value].SetIconVisible(true);
        }
        draggingSlotIndex = null;

        if (dragIcon != null)
        {
            dragIcon.gameObject.SetActive(false);
        }
    }
}
