using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LegendElement : MonoBehaviour
{
    public void SetElement(Texture2D tex, string name)
    {
        GetComponentInChildren<RawImage>().texture = tex;
        GetComponentInChildren<TextMeshProUGUI>().text = name;
    }
}
