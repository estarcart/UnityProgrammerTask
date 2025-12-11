using UnityEngine;
using System;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    public event Action<NotificationData> OnNotificationRequested;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowNotification(string message, NotificationType type = NotificationType.Info, float duration = 3f)
    {
        var data = new NotificationData(message, type, duration);
        OnNotificationRequested?.Invoke(data);
    }

    public void ShowNotification(NotificationData data)
    {
        OnNotificationRequested?.Invoke(data);
    }

    public void ShowWarning(string message, float duration = 3f)
    {
        ShowNotification(message, NotificationType.Warning, duration);
    }

    public void ShowError(string message, float duration = 3f)
    {
        ShowNotification(message, NotificationType.Error, duration);
    }

    public void ShowSuccess(string message, float duration = 3f)
    {
        ShowNotification(message, NotificationType.Success, duration);
    }

    public void ShowInfo(string message, float duration = 3f)
    {
        ShowNotification(message, NotificationType.Info, duration);
    }
}
