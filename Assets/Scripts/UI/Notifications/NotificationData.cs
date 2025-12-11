using UnityEngine;

public enum NotificationType
{
    Info,
    Warning,
    Error,
    Success
}

[System.Serializable]
public class NotificationData
{
    public string message;
    public NotificationType type;
    public float duration;
    public Color? customColor;

    public NotificationData(string message, NotificationType type = NotificationType.Info, float duration = 3f, Color? customColor = null)
    {
        this.message = message;
        this.type = type;
        this.duration = duration;
        this.customColor = customColor;
    }
}
