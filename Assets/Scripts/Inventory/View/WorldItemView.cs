using UnityEngine;

public class WorldItemView : MonoBehaviour
{
    [SerializeField] private string itemId;
    [SerializeField] private int amount = 1;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public string ItemId => itemId;
    public int Amount => amount;

    public void SetItem(string itemId, int amount, Sprite icon = null)
    {
        this.itemId = itemId;
        this.amount = amount;
        
        if (icon != null && spriteRenderer != null)
            spriteRenderer.sprite = icon;
    }

    public ItemInstance ToItemInstance()
    {
        return new ItemInstance(itemId, amount);
    }
}
