using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text amountText;

    private int slotIndex;
    private InventoryView inventoryView;

    public int SlotIndex => slotIndex;

    public void Initialize(int index, InventoryView view)
    {
        slotIndex = index;
        inventoryView = view;
    }

    public void SetEmpty()
    {
        iconImage.enabled = false;
        amountText.text = string.Empty;
    }

    public void SetItem(Sprite icon, int amount, bool stackable)
    {
        iconImage.enabled = true;
        iconImage.sprite = icon;
        amountText.text = stackable && amount > 1 ? amount.ToString() : string.Empty;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            inventoryView.NotifySlotRightClicked(slotIndex, eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryView.NotifySlotHoverEnter(slotIndex, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryView.NotifySlotHoverExit(slotIndex, eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        inventoryView.NotifyBeginDrag(slotIndex, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        inventoryView.NotifyDrag(slotIndex, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryView.NotifyEndDrag(slotIndex, eventData);
    }
}
