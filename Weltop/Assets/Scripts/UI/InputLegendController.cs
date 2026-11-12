using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;
using UnityEngine.Windows;

public class InputLegendController : MonoBehaviour
{
    public ViewSwitcher viewSwitcher;
    public GameObject element;
    public IconDict XboxDict;
    public IconDict PlaystationDict;
    public IconDict KeyBoardDict;
    private IconDict curDict;
    private float height;
    private int MaxElements;
    private GameObject[] elementList;
    private Action onBuildMode;
    private Action onParkorMode;
    private Action onRagdollMode;
    private PlayerInput input;
    void Start()
    {
        input = viewSwitcher.GetPlayerInput();
        SetDict(input);
        viewSwitcher.OnBuildMode += ShowLegend;
        viewSwitcher.OnBuildMode += BuildMode;
        viewSwitcher.OnParkor += ShowLegend;
        viewSwitcher.OnParkor += ParkorMode;
        viewSwitcher.OnRagdoll += HideLegend;
        height = ((RectTransform)element.transform).rect.height;
        MaxElements = (int)Mathf.Floor(((RectTransform)transform).rect.height/height);
        elementList = new GameObject[MaxElements];
    }

    private void OnDestroy()
    {

        viewSwitcher.OnBuildMode -= ShowLegend;
        viewSwitcher.OnBuildMode -= BuildMode;
        viewSwitcher.OnParkor -= ShowLegend;
        viewSwitcher.OnParkor -= ParkorMode;
        viewSwitcher.OnRagdoll -= HideLegend;
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
            Debug.Log("keybourd detected");
            curDict = KeyBoardDict;
        }
    }

    private int curIndex;
    public void CreateElement(InputAction action)
    {
        if (curDict == null) { Debug.LogError("There is no chosen Icon dictionary"); return; }
        if (curIndex == MaxElements) { Debug.LogError("Max Elements in legend"); return; }
        Debug.Log(height);
        elementList[curIndex] = Instantiate(element,transform);
        RectTransform t = (RectTransform)elementList[curIndex].transform;
        t.anchoredPosition = new Vector2(0, ((MaxElements / 2) - curIndex) * height - height / 2);
        elementList[curIndex].GetComponent<LegendElement>().SetElement(curDict.GetIcon(action),action.name);
        curIndex++;
    }
    public void CreateElement(InputAction action,string name)
    {
        if (curDict == null) { Debug.LogError("There is no chosen Icon dictionary"); return; }
        if (curIndex == MaxElements) { Debug.LogError("Max Elements in legend"); return; }

        elementList[curIndex] = Instantiate(element, transform);
        elementList[curIndex].GetComponent<LegendElement>().SetElement(curDict.GetIcon(action), name);
        RectTransform t = (RectTransform)elementList[curIndex].transform;
        t.anchoredPosition = new Vector2(0, ( (MaxElements / 2) - curIndex) * height - height/2);
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

    public void HideLegend()
    {
        GetComponent<Image>().enabled = false;
        foreach (Transform tr in GetComponentsInChildren<Transform>())
        {
            if (tr != transform)
            {
                tr.gameObject.SetActive(false);
            }
        }
    }
    public void ShowLegend()
    {

        GetComponent<Image>().enabled = true;
        foreach (Transform tr in GetComponentsInChildren<Transform>())
        {
            if (tr != transform)
            {
                tr.gameObject.SetActive(true);
            }
        }
    }

    public void BuildMode()
    {
        ResetElements();
        CreateElement(input.actions["Place"]);
        CreateElement(input.actions["Rotate"]);
        CreateElement(input.actions["Move"]);
        CreateElement(input.actions["Jump"],"Move Up");
        CreateElement(input.actions["Crouch"],"Move Down");
    }
    public void ParkorMode()
    {
        ResetElements();
        CreateElement(input.actions["Move"]);
        CreateElement(input.actions["Jump"]);
        CreateElement(input.actions["Attack"]);
    }

}
