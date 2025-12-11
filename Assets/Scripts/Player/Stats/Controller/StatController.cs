using UnityEngine;
using System;

public class StatController : MonoBehaviour
{
    [SerializeField] private StatDefinition statDefinition;
    [SerializeField] private StatBarView statBarView;

    private StatModel statModel;
    private float timeSinceLastDamage;
    private bool canRegenerate;

    public StatModel Model => statModel;
    public float CurrentValue => statModel?.CurrentValue ?? 0f;
    public float MaxValue => statModel?.MaxValue ?? 0f;
    public float Percentage => statModel?.Percentage ?? 0f;
    public bool IsDepleted => statModel?.IsDepleted ?? false;
    public bool IsFull => statModel?.IsFull ?? true;

    public event Action<float, float> OnValueChanged;
    public event Action OnDepleted;
    public event Action OnFull;

    void Awake()
    {
        if (statDefinition != null)
        {
            InitializeFromDefinition(statDefinition);
        }
    }

    public void InitializeFromDefinition(StatDefinition definition)
    {
        statDefinition = definition;
        statModel = new StatModel(definition.maxValue, definition.minValue, definition.startValue);

        statModel.OnValueChanged += HandleValueChanged;
        statModel.OnDepleted += HandleDepleted;
        statModel.OnFull += HandleFull;

        if (statBarView != null)
        {
            statBarView.Initialize(definition.barColor);
            statBarView.UpdateDisplay(statModel.CurrentValue, statModel.MaxValue);
        }

        canRegenerate = true;
        timeSinceLastDamage = definition.regenDelay;
    }

    void OnDestroy()
    {
        if (statModel != null)
        {
            statModel.OnValueChanged -= HandleValueChanged;
            statModel.OnDepleted -= HandleDepleted;
            statModel.OnFull -= HandleFull;
        }
    }

    void Update()
    {
        HandleRegeneration();
    }

    private void HandleRegeneration()
    {
        if (statDefinition == null || !statDefinition.regenerates || statModel == null)
            return;

        if (statModel.IsFull)
            return;

        timeSinceLastDamage += Time.deltaTime;

        if (canRegenerate && timeSinceLastDamage >= statDefinition.regenDelay)
        {
            statModel.Add(statDefinition.regenRate * Time.deltaTime);
        }
    }

    private void HandleValueChanged(float current, float max)
    {
        statBarView?.UpdateDisplay(current, max);
        OnValueChanged?.Invoke(current, max);
    }

    private void HandleDepleted()
    {
        OnDepleted?.Invoke();
    }

    private void HandleFull()
    {
        OnFull?.Invoke();
    }

    public void Add(float amount)
    {
        statModel?.Add(amount);
    }

    public void Remove(float amount)
    {
        if (statModel == null) return;
        statModel.Remove(amount);
        timeSinceLastDamage = 0f;
    }

    public void SetValue(float value)
    {
        statModel?.SetValue(value);
    }

    public void Refill()
    {
        statModel?.Refill();
    }
}
