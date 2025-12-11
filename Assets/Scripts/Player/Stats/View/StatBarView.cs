using UnityEngine;
using UnityEngine.UI;

public class StatBarView : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Text valueText;
    [SerializeField] private GameObject container;

    [Header("Animation")]
    [SerializeField] private bool animateChanges = true;
    [SerializeField] private float animationSpeed = 5f;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.red;
    [SerializeField] private Color lowColor = Color.yellow;
    [SerializeField] private float lowThreshold = 0.25f;

    private float targetFillAmount;
    private float currentFillAmount;

    public void Initialize(Color barColor)
    {
        normalColor = barColor;
        if (fillImage != null)
        {
            fillImage.color = normalColor;
        }
        targetFillAmount = 1f;
        currentFillAmount = 1f;
    }

    public void UpdateDisplay(float current, float max)
    {
        if (max <= 0) return;

        targetFillAmount = current / max;

        if (valueText != null)
        {
            valueText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
        }

        if (fillImage != null)
        {
            fillImage.color = targetFillAmount <= lowThreshold ? lowColor : normalColor;
        }

        if (!animateChanges)
        {
            currentFillAmount = targetFillAmount;
            if (fillImage != null)
            {
                fillImage.fillAmount = currentFillAmount;
            }
        }
    }

    void Update()
    {
        if (animateChanges && fillImage != null)
        {
            if (Mathf.Abs(currentFillAmount - targetFillAmount) > 0.001f)
            {
                currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * animationSpeed);
                fillImage.fillAmount = currentFillAmount;
            }
        }
    }

    public void Show()
    {
        if (container != null)
        {
            container.SetActive(true);
        }
    }

    public void Hide()
    {
        if (container != null)
        {
            container.SetActive(false);
        }
    }
}
