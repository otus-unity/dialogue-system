using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NotificationManager : INotificationManager
{
    public void DisplayMessage(string message)
    {
        Debug.Log(message);
    }
}
