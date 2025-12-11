using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinition", menuName = "Scriptable Objects/ItemDefinition")]
public class ItemDefinition : ScriptableObject
{
    public string id;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public bool stackable = true;
    public int maxStack = 99;

    [Header("Item Usage")]
    [Tooltip("If true, this item can be used from the hotbar")]
    public bool isUsable = false;
    
    [Tooltip("The effect to apply when this item is used. Leave empty if not usable.")]
    public ItemEffect useEffect;
}
