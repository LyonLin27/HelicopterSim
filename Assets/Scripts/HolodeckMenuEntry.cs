using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HolodeckMenuEntry : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Transform arrow;

    public void SetupView(HolodeckMenuEntryData data)
    {
        text.text = data.display;
        arrow.gameObject.SetActive(!data.isLeaf);
    }

    public void SetHighlight(bool b)
    {
        GetComponent<Image>().enabled = b;
    }
}
