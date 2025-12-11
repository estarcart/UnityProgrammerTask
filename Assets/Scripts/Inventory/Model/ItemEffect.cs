using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract bool Apply(GameObject user, ItemInstance itemInstance, ItemDefinition itemDefinition);
}
