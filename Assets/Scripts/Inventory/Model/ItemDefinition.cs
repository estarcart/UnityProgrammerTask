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
}
