using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HotbarSlotView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text amountText;
    [SerializeField] private TextMeshProUGUI hotkeyText;
    [SerializeField] private GameObject selectionIndicator;
    [SerializeField] private Image slotBackground;

    [Header("Selection Colors")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    [SerializeField] private Color selectedColor = new Color(0.4f, 0.6f, 0.8f, 0.9f);

    private int slotIndex;
    private HotbarView hotbarView;
    private bool isSelected;

    public int SlotIndex => slotIndex;

    public Sprite GetIcon()
    {
        return iconImage != null && iconImage.enabled ? iconImage.sprite : null;
    }

    public void SetIconVisible(bool visible)
    {
        if (iconImage != null && iconImage.sprite != null)
        {
            iconImage.color = visible ? Color.white : Color.clear;
        }
        if (amountText != null)
        {
            amountText.color = visible ? Color.white : Color.clear;
        }
    }

    public void Initialize(int index, HotbarView view, string hotkeyLabel)
    {
        slotIndex = index;
        hotbarView = view;

        if (hotkeyText != null)
        {
            hotkeyText.text = hotkeyLabel;
        }

        SetSelected(false);
    }

    public void SetEmpty()
    {
        if (iconImage != null)
        {
            iconImage.enabled = false;
        }

        if (amountText != null)
        {
            amountText.text = string.Empty;
        }
    }

    public void SetItem(Sprite icon, int amount, bool stackable)
    {
        if (iconImage != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = icon;
        }

        if (amountText != null)
        {
            amountText.text = stackable && amount > 1 ? amount.ToString() : string.Empty;
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(selected);
        }

        if (slotBackground != null)
        {
            slotBackground.color = selected ? selectedColor : normalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            hotbarView.NotifySlotClicked(slotIndex, eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            hotbarView.NotifySlotRightClicked(slotIndex, eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hotbarView.NotifySlotHoverEnter(slotIndex, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hotbarView.NotifySlotHoverExit(slotIndex, eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        hotbarView.NotifyBeginDrag(slotIndex, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        hotbarView.NotifyDrag(slotIndex, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        hotbarView.NotifyEndDrag(slotIndex, eventData);
    }
}
