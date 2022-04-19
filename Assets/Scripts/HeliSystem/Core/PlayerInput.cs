using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput
{
    public PlayerActions actions;

    // raw input
    public float acc_in;
    public float dcc_in;
    public float rot_in;
    public Vector2 tlt_in;
    public Vector2 cam_in;
    public bool mg_in;
    public bool ms_in;
    public bool aim_in;
    public Vector2 menu_in = Vector2.zero;

    // parsed input
    public float acc_parsed = 0;
    public float dcc_parsed = 0;
    public float rot_parsed = 0;
    public Vector2 tlt_parsed = Vector2.zero;
    public bool isStablized = true;

    public PlayerInput()
    {
        actions = new PlayerActions();
        actions.Enable();
        actions.HeliController.Acc.performed += ctx => acc_in = ctx.ReadValue<float>();
        actions.HeliController.Dcc.performed += ctx => dcc_in = ctx.ReadValue<float>();
        actions.HeliController.Rotate.performed += ctx => rot_in = ctx.ReadValue<float>();
        actions.HeliController.Tilt.performed += ctx => tlt_in = ctx.ReadValue<Vector2>();
        actions.HeliController.CamControl.performed += ctx => cam_in = ctx.ReadValue<Vector2>();
        actions.HeliController.Unlock.performed += ctx => isStablized = false;
        actions.HeliController.MachineGun.performed += ctx => mg_in = true;
        actions.HeliController.Missile.performed += ctx => ms_in = true;
        actions.HeliController.Aim.performed += ctx => aim_in = true;

        actions.HeliController.MenuAction.performed += ctx => menu_in = ctx.ReadValue<Vector2>();

        actions.HeliController.Acc.canceled += ctx => acc_in = 0f;
        actions.HeliController.Dcc.canceled += ctx => dcc_in = 0f;
        actions.HeliController.Rotate.canceled += ctx => rot_in = 0f;
        actions.HeliController.Tilt.canceled += ctx => tlt_in = Vector2.zero;
        actions.HeliController.CamControl.canceled += ctx => cam_in = Vector2.zero;
        actions.HeliController.Unlock.canceled += ctx => isStablized = true;
        actions.HeliController.MachineGun.canceled += ctx => mg_in = false;
        actions.HeliController.Missile.canceled += ctx => ms_in = false;
        actions.HeliController.Aim.canceled += ctx => aim_in = false;

        if (PlayerPrefs.HasKey("SavedAxisControl"))
            RebindAxis((AxisControlType)PlayerPrefs.GetInt("SavedAxisControl"));
    }

    public void Destroy()
    {
        actions.Disable();
        actions.Dispose();
    }

    public void Tick()
    {
        acc_parsed = Mathf.Lerp(acc_parsed, acc_in, Time.deltaTime * 20f);
        dcc_parsed = Mathf.Lerp(dcc_parsed, dcc_in, Time.deltaTime * 20f);
        rot_parsed = Mathf.Lerp(rot_parsed, rot_in, Time.deltaTime * 20f);
        tlt_parsed = Vector2.Lerp(tlt_parsed, tlt_in, Time.deltaTime * 20f);
    }

    public enum AxisControlType 
    {
        type0,
        type1,
        type2,
        type3,
    }

    public void RebindAxis(AxisControlType type)
    {
        switch (type)
        {
            case AxisControlType.type0:
                actions.HeliController.Rotate.ApplyBindingOverride("<Gamepad>/rightStick/x");
                actions.HeliController.Tilt.ApplyBindingOverride("<Gamepad>/leftStick");
                break;
            case AxisControlType.type1:
                actions.HeliController.Rotate.ApplyBindingOverride("<Gamepad>/leftStick/x");
                actions.HeliController.Tilt.ApplyBindingOverride("<Gamepad>/rightStick");
                break;
            case AxisControlType.type2:
                break;
            case AxisControlType.type3:
                break;
        }
    }

    public AxisControlType GetCurrentAxisType()
    {
        if (actions.HeliController.Tilt.bindings[0].effectivePath.Contains("left"))
            return AxisControlType.type0;
        else
            return AxisControlType.type1;
    }
}
