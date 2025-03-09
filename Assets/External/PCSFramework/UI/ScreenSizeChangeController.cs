using System;
using UnityEngine;

public static class ScreenSizeChangeController
{
    public static event Action<Vector2> OnChanged;

    public static void Notify()
    {
        OnChanged?.Invoke(new Vector2(Screen.width, Screen.height));
    }
}
