using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

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
        Texture2D tex = Icons[GetBinding(action).path.Split('/', 2)[1]];
        if (tex == null) { Debug.LogError("Cant find " + GetBinding(action).path.Split('/', 2)[1] + " in " + this.name); }
        return tex;
    }


}
