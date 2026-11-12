using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

[CreateAssetMenu(fileName = "IconDict", menuName = "Scriptable Objects/IconDict")]
public class IconDict : ScriptableObject
{
    public string Device;
    [SerializeField]
    public Dictionary<string, Texture2D> Icons;
    public InputBinding GetBinding(InputAction action)
    {
        foreach(InputBinding binding in action.bindings)
        {
            if ((binding.path.Split('/')[0]) == Device)
            {
                return binding;
            }
        }

        Debug.LogError("Could not find binding for " + action.name + " for " + Device);
        return new InputBinding();
    }

    public Texture2D GetIcon(InputAction action)
    {
        return Icons[GetBinding(action).path.Split('/',2)[1]];
    }


}
