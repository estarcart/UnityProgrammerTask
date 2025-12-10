using UnityEngine;

public class WorldItemView : MonoBehaviour
{
    [SerializeField] private string itemId;
    [SerializeField] private int amount = 1;

    public string ItemId => itemId;
    public int Amount => amount;

    public void SetItem(string itemId, int amount)
    {
        this.itemId = itemId;
        this.amount = amount;
    }

    public ItemInstance ToItemInstance()
    {
        return new ItemInstance(itemId, amount);
    }
}
