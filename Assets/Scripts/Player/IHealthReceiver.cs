public interface IHealthReceiver
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsFullHealth { get; }
    void Heal(float amount);
}
