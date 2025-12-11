using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject notificationRoot;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Type Colors")]
    [SerializeField] private Color infoColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color warningColor = new Color(1f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color errorColor = new Color(1f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color successColor = new Color(0.3f, 1f, 0.5f, 1f);

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.8f;

    private Coroutine currentNotificationCoroutine;

    private void Start()
    {
        if (notificationRoot != null)
        {
            notificationRoot.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        SubscribeToNotifications();
    }

    private void OnEnable()
    {
        SubscribeToNotifications();
    }

    private void OnDisable()
    {
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.OnNotificationRequested -= HandleNotification;
        }
    }

    private void SubscribeToNotifications()
    {
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.OnNotificationRequested -= HandleNotification;
            NotificationManager.Instance.OnNotificationRequested += HandleNotification;
        }
    }

    private void HandleNotification(NotificationData data)
    {
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine);
        }

        currentNotificationCoroutine = StartCoroutine(ShowNotificationCoroutine(data));
    }

    private IEnumerator ShowNotificationCoroutine(NotificationData data)
    {
        notificationRoot.SetActive(true);
        messageText.text = data.message;
        messageText.color = data.customColor ?? GetColorForType(data.type);

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        float displayTime = data.duration - fadeInDuration - fadeOutDuration;
        if (displayTime > 0)
        {
            yield return new WaitForSeconds(displayTime);
        }

        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        notificationRoot.SetActive(false);
        currentNotificationCoroutine = null;
    }

    private Color GetColorForType(NotificationType type)
    {
        return type switch
        {
            NotificationType.Warning => warningColor,
            NotificationType.Error => errorColor,
            NotificationType.Success => successColor,
            _ => infoColor
        };
    }
}
