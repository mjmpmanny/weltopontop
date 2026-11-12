using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class InputLegendController : MonoBehaviour
{
    
    public GameObject element;
    public IconDict XboxDict;
    public IconDict PlaystationDict;
    public IconDict KeyBoardDict;
    private IconDict curDict;
    private float height;
    private int MaxElements;
    private GameObject[] elementList;
    void Start()
    {
        height = ((RectTransform)element.transform).rect.height;
        MaxElements = (int)Mathf.Floor(((RectTransform)transform).rect.height/height);
        elementList = new GameObject[MaxElements];
    }

    public void SetDict(PlayerInput inp)
    {
        if (inp.devices[0] is XInputController)
        {
            curDict = XboxDict;
        } else if (inp.devices[0] is DualShockGamepad)
        {
            curDict = PlaystationDict;
        } else if (inp.devices[0] is Keyboard)
        {
            curDict = KeyBoardDict;
        }
    }

    private int curIndex;
    public void CreateElement(InputAction action)
    {
        if (curDict == null) { Debug.LogError("There is no chosen Icon dictionary"); return; }
        if (curIndex == MaxElements) { Debug.LogError("Max Elements in legend"); return; }

        elementList[curIndex] = Instantiate(element,transform);
        elementList[curIndex].GetComponent<LegendElement>().SetElement(curDict.GetIcon(action),action.name);
        curIndex++;
    }

    public void ResetElements()
    {
        foreach(GameObject g in elementList)
        {
            Destroy(g);
        }
        curIndex = 0;
    }

}
