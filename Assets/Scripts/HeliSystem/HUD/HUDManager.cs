using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public LockOnSystem lockOnSystem;

    [SerializeField]
    private DebugInfoPanel debugInfoPanel;

    public void SetDebugInfo(int index, string info)
    {
        if (debugInfoPanel.debugTexts.Count <= index)
            return;

        debugInfoPanel.debugTexts[index].text = info;
    }
}
