using System;

public class StatModel
{
    public event Action<float, float> OnValueChanged;
    public event Action OnDepleted;
    public event Action OnFull;

    private float currentValue;
    private float maxValue;
    private float minValue;

    public float CurrentValue => currentValue;
    public float MaxValue => maxValue;
    public float MinValue => minValue;
    public float Percentage => maxValue > 0 ? currentValue / maxValue : 0f;
    public bool IsDepleted => currentValue <= minValue;
    public bool IsFull => currentValue >= maxValue;

    public StatModel(float maxValue, float minValue = 0f, float? startValue = null)
    {
        this.maxValue = maxValue;
        this.minValue = minValue;
        this.currentValue = startValue ?? maxValue;
    }

    public void Add(float amount)
    {
        if (amount <= 0) return;

        float previousValue = currentValue;
        currentValue = Math.Min(currentValue + amount, maxValue);

        if (currentValue != previousValue)
        {
            OnValueChanged?.Invoke(currentValue, maxValue);

            if (IsFull)
            {
                OnFull?.Invoke();
            }
        }
    }

    public void Remove(float amount)
    {
        if (amount <= 0) return;

        float previousValue = currentValue;
        currentValue = Math.Max(currentValue - amount, minValue);

        if (currentValue != previousValue)
        {
            OnValueChanged?.Invoke(currentValue, maxValue);

            if (IsDepleted)
            {
                OnDepleted?.Invoke();
            }
        }
    }

    public void SetValue(float value)
    {
        float previousValue = currentValue;
        currentValue = Math.Clamp(value, minValue, maxValue);

        if (currentValue != previousValue)
        {
            OnValueChanged?.Invoke(currentValue, maxValue);

            if (IsDepleted)
            {
                OnDepleted?.Invoke();
            }
            else if (IsFull)
            {
                OnFull?.Invoke();
            }
        }
    }

    public void SetMax(float newMax, bool adjustCurrent = true)
    {
        maxValue = Math.Max(newMax, minValue);
        
        if (adjustCurrent && currentValue > maxValue)
        {
            currentValue = maxValue;
        }

        OnValueChanged?.Invoke(currentValue, maxValue);
    }

    public void Refill()
    {
        SetValue(maxValue);
    }

    public void Deplete()
    {
        SetValue(minValue);
    }
}
