using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolodeckMenuEntryData
{
    public string key;
    public string display;
    private Action OnSelect;
    private Func<int> GetDefaultIndex;
    public HolodeckMenuEntryData parent;
    public List<HolodeckMenuEntryData> content;
    public bool isLeaf;
    public int currIndex = 0;

    public HolodeckMenuEntryData(string _key,
                        string _display = "",
                        List<HolodeckMenuEntryData> _content = null,
                        Func<int> _getDefaultIndex = null)
    {
        key = _key;
        display = (_display == "") ? _key : _display;
        content = _content;
        GetDefaultIndex = _getDefaultIndex;
        isLeaf = false;
    }

    public HolodeckMenuEntryData(string _key,
                        string _display = "",
                        Action _onSelect = null)
    {
        key = _key;
        display = (_display == "") ? _key : _display;
        OnSelect = _onSelect;
        isLeaf = true;
    }

    public void SetParent(HolodeckMenuEntryData _parent)
    {
        parent = _parent;
        parent.content.Add(this);
    }

    public void Select()
    {
        if (OnSelect != null)
            OnSelect();
    }

    public void SelectNext()
    {
        currIndex++;
        if (currIndex >= content.Count)
            currIndex = 0;
    }

    public void SelectPrev()
    {
        currIndex--;
        if (currIndex < 0)
            currIndex = content.Count-1;
    }

    public void RefreshDefaultIndex()
    {
        if (GetDefaultIndex == null)
        {
            currIndex = 0;
            return;
        }

        currIndex = GetDefaultIndex();
    }
}

public class HolodeckMenu : MonoBehaviour
{
    public List<Transform> entryTransList;

    private HolodeckMenuEntryData menuRoot;
    private HolodeckMenuEntryData currDisplayNode;

    private PlayerInput pi;
    private Action menuActionRef;

	private void Start()
	{
        InitTreeMenu();
        SetupView();

        pi = GameManager.instance.playerInput;
        pi.actions.HeliController.MenuAction.performed += ctx => HandleMenuInput(ctx.ReadValue<Vector2>());
    }

    private void OnDestroy()
    {
        pi.actions.HeliController.MenuAction.performed -= ctx => HandleMenuInput(ctx.ReadValue<Vector2>());
    }

    public void SetupView()
    {
        if (currDisplayNode == null)
            currDisplayNode = menuRoot;

        for (int i = 0; i < entryTransList.Count; i++)
        {
            if (i < currDisplayNode.content.Count)
            {
                entryTransList[i].gameObject.SetActive(true);
                entryTransList[i].GetComponent<HolodeckMenuEntry>().SetupView(currDisplayNode.content[i]);
                entryTransList[i].GetComponent<HolodeckMenuEntry>().SetHighlight(currDisplayNode.currIndex == i);
            }
            else 
            {
                entryTransList[i].gameObject.SetActive(false);
            }
        }
    }

    private void InitTreeMenu()
    {
        currDisplayNode = null;
        menuRoot = new HolodeckMenuEntryData("root", "", new List<HolodeckMenuEntryData>());

        var controlSettingEntry = new HolodeckMenuEntryData(
            "ControlSettings", 
            "Control Settings", 
            new List<HolodeckMenuEntryData>(),
            GetControlSettingIndex
        );
        var optionAEntry = new HolodeckMenuEntryData(
            "ControlOptionA", 
            "L Stick Tilt, R Stick Rotate", 
            () => GameManager.instance.SelectControlType(PlayerInput.AxisControlType.type0)
        );
        var optionBEntry = new HolodeckMenuEntryData(
            "ControlOptionB", 
            "R Stick Tilt, L Stick Rotate", 
            () => GameManager.instance.SelectControlType(PlayerInput.AxisControlType.type1)
        );

        optionAEntry.SetParent(controlSettingEntry);
        optionBEntry.SetParent(controlSettingEntry);
        controlSettingEntry.SetParent(menuRoot);

        var levelSettingEntry = new HolodeckMenuEntryData(
            "LevelSelect", 
            "Select Level", 
            new List<HolodeckMenuEntryData>(),
            GetLevelIndex
        );
        var tutLevelEntry = new HolodeckMenuEntryData(
            "TutLevel", 
            "Practice", 
            () => GameManager.instance.SelectLevel(GameManager.LevelType.practice)
        );
            var cityLevelEntry = new HolodeckMenuEntryData(
            "CityLevel", 
            "City", 
            () => GameManager.instance.SelectLevel(GameManager.LevelType.city)
        );

        tutLevelEntry.SetParent(levelSettingEntry);
        cityLevelEntry.SetParent(levelSettingEntry);
        levelSettingEntry.SetParent(menuRoot);
    }

    bool inputCD = false;
    float lastInputTime = 0f;

    public void HandleMenuInput(Vector2 input)
    {
        if (Time.time - lastInputTime > 0.1f)
            inputCD = false;

        if (inputCD)
            return;

        if (input.x > 0.5f)
            Advance();
        else if (input.x < -0.5f)
            Back();
        else if (input.y > 0.5f)
            MoveUp();
        else if (input.y < -0.5f)
            MoveDown();

        inputCD = true;
        lastInputTime = Time.time;
    }

    public void Advance() 
    {
        if (currDisplayNode.content[currDisplayNode.currIndex].isLeaf)
            currDisplayNode.content[currDisplayNode.currIndex].Select();
        else 
        {
            currDisplayNode = currDisplayNode.content[currDisplayNode.currIndex];
            SetupView();
        }
    }

    public void Back()
    {
        if (currDisplayNode != menuRoot)
        {
            currDisplayNode = currDisplayNode.parent;
            SetupView();
        }
    }

    public void MoveUp() 
    {
        currDisplayNode.SelectNext();
        SetupView();
    }

    public void MoveDown()
    {
        currDisplayNode.SelectPrev();
        SetupView();
    }

    private int GetLevelIndex()
    {
        GameManager.LevelType currLevel = GameManager.instance.currLevel;
        if (currLevel == GameManager.LevelType.city)
            return 1;
        else
            return 0;
    }

    private int GetControlSettingIndex()
    {
        PlayerInput.AxisControlType currType = GameManager.instance.playerInput.GetCurrentAxisType();
        if (currType == PlayerInput.AxisControlType.type0)
            return 0;
        else
            return 1;
    }
}
