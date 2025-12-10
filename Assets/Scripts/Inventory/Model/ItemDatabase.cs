using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemDefinition> items = new();

    private Dictionary<string, ItemDefinition> dict;

    void OnEnable()
    {
        dict = new Dictionary<string, ItemDefinition>();
        foreach (var item in items)
        {
            if (item != null && !string.IsNullOrEmpty(item.id))
            {
                dict[item.id] = item;
            }
        }
    }

    public ItemDefinition GetItem(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        dict ??= new Dictionary<string, ItemDefinition>();
        if (dict.TryGetValue(id, out var def))
            return def;
        return null;
    }
}
