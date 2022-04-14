using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HolodeckUIManager : MonoBehaviour
{
    public HolodeckMenu treeMenu;
    public Transform levelInfo;

    public void SetLevelInfo(string text)
    {
        levelInfo.GetComponent<TextMeshProUGUI>().text = text;
    }
}
