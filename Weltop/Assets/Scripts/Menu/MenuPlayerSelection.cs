using System.Collections.Generic;
using UnityEngine.InputSystem;

public static class MenuPlayerSelection
{
    public static readonly List<InputDevice> SelectedDevices = new();

    public static void SetPlayers(IEnumerable<InputDevice> devices)
    {
        SelectedDevices.Clear();
        SelectedDevices.AddRange(devices);
    }

    public static void Clear()
    {
        SelectedDevices.Clear();
    }
}