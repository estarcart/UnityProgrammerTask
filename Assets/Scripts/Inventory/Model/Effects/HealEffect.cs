using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Scriptable Objects/Item Effects/Heal Effect")]
public class HealEffect : ItemEffect
{
    [SerializeField] private int healAmount = 25;

    public override bool Apply(GameObject user, ItemInstance itemInstance, ItemDefinition itemDefinition)
    {
        var health = user.GetComponent<IHealthReceiver>();
        
        if (health == null) return false;
        if (health.IsFullHealth) return false;

        health.Heal(healAmount);
        return true;
    }
}
