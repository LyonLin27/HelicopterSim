// GENERATED AUTOMATICALLY FROM 'Assets/InputActions/PlayerActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerActions"",
    ""maps"": [
        {
            ""name"": ""HeliController"",
            ""id"": ""cd8825f7-a10e-4b45-bc4c-f4c7c06a4e9d"",
            ""actions"": [
                {
                    ""name"": ""Acc"",
                    ""type"": ""Value"",
                    ""id"": ""e8f02e21-1449-428a-900a-fb33494d38d8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dcc"",
                    ""type"": ""Value"",
                    ""id"": ""e8e2d608-aa7e-4a8e-be29-5c2081d64882"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Tilt"",
                    ""type"": ""Value"",
                    ""id"": ""192469ad-836b-4921-a0c8-d4b28e08535b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""2aad22ef-95ff-4235-8c4b-634d174a1a55"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Unlock"",
                    ""type"": ""Button"",
                    ""id"": ""804a6bf4-688b-4616-9b3f-f6645e388734"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CamControl"",
                    ""type"": ""Value"",
                    ""id"": ""cc872eef-7617-476f-8d2b-54866cb15c53"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MachineGun"",
                    ""type"": ""Button"",
                    ""id"": ""2538539b-59e0-4bcf-8787-063eb6c52bbf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Missile"",
                    ""type"": ""Button"",
                    ""id"": ""8cbbba25-d09c-4d02-8056-15bede0525c9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MenuAction"",
                    ""type"": ""Value"",
                    ""id"": ""4f61e4be-18cb-42ac-b3a9-abbd0d773560"",
                    ""expectedControlType"": ""Dpad"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bc17b086-068a-4dfa-b8bc-243fae676ee0"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Acc"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0117bdd7-7646-4162-80b5-7893f38bd3a6"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Dcc"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""94212096-c915-42c7-952a-b81b05233b4a"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Tilt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1efe268a-dfa8-445d-a139-376c3b3d25d3"",
                    ""path"": ""<Gamepad>/rightStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0516dc1d-ade1-46a4-8bbb-83a9e4709d08"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Unlock"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7d094f9c-3102-4d5b-ba3a-88e6db70253b"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""CamControl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f75a8813-2ea5-48e5-8243-98b7be2cac94"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""MachineGun"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b9da3435-7a82-4e88-8df7-266cbeae6892"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Missile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b27e791-6fbd-434b-95b2-4cab9e1b5cb1"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // HeliController
        m_HeliController = asset.FindActionMap("HeliController", throwIfNotFound: true);
        m_HeliController_Acc = m_HeliController.FindAction("Acc", throwIfNotFound: true);
        m_HeliController_Dcc = m_HeliController.FindAction("Dcc", throwIfNotFound: true);
        m_HeliController_Tilt = m_HeliController.FindAction("Tilt", throwIfNotFound: true);
        m_HeliController_Rotate = m_HeliController.FindAction("Rotate", throwIfNotFound: true);
        m_HeliController_Unlock = m_HeliController.FindAction("Unlock", throwIfNotFound: true);
        m_HeliController_CamControl = m_HeliController.FindAction("CamControl", throwIfNotFound: true);
        m_HeliController_MachineGun = m_HeliController.FindAction("MachineGun", throwIfNotFound: true);
        m_HeliController_Missile = m_HeliController.FindAction("Missile", throwIfNotFound: true);
        m_HeliController_MenuAction = m_HeliController.FindAction("MenuAction", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // HeliController
    private readonly InputActionMap m_HeliController;
    private IHeliControllerActions m_HeliControllerActionsCallbackInterface;
    private readonly InputAction m_HeliController_Acc;
    private readonly InputAction m_HeliController_Dcc;
    private readonly InputAction m_HeliController_Tilt;
    private readonly InputAction m_HeliController_Rotate;
    private readonly InputAction m_HeliController_Unlock;
    private readonly InputAction m_HeliController_CamControl;
    private readonly InputAction m_HeliController_MachineGun;
    private readonly InputAction m_HeliController_Missile;
    private readonly InputAction m_HeliController_MenuAction;
    public struct HeliControllerActions
    {
        private @PlayerActions m_Wrapper;
        public HeliControllerActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Acc => m_Wrapper.m_HeliController_Acc;
        public InputAction @Dcc => m_Wrapper.m_HeliController_Dcc;
        public InputAction @Tilt => m_Wrapper.m_HeliController_Tilt;
        public InputAction @Rotate => m_Wrapper.m_HeliController_Rotate;
        public InputAction @Unlock => m_Wrapper.m_HeliController_Unlock;
        public InputAction @CamControl => m_Wrapper.m_HeliController_CamControl;
        public InputAction @MachineGun => m_Wrapper.m_HeliController_MachineGun;
        public InputAction @Missile => m_Wrapper.m_HeliController_Missile;
        public InputAction @MenuAction => m_Wrapper.m_HeliController_MenuAction;
        public InputActionMap Get() { return m_Wrapper.m_HeliController; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(HeliControllerActions set) { return set.Get(); }
        public void SetCallbacks(IHeliControllerActions instance)
        {
            if (m_Wrapper.m_HeliControllerActionsCallbackInterface != null)
            {
                @Acc.started -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnAcc;
                @Acc.performed -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnAcc;
                @Acc.canceled -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnAcc;
                @Dcc.started -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnDcc;
                @Dcc.performed -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnDcc;
                @Dcc.canceled -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnDcc;
                @Tilt.started -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnTilt;
                @Tilt.performed -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnTilt;
                @Tilt.canceled -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnTilt;
                @Rotate.started -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnRotate;
                @Unlock.started -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnUnlock;
                @Unlock.performed -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnUnlock;
                @Unlock.canceled -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnUnlock;
                @CamControl.started -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnCamControl;
                @CamControl.performed -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnCamControl;
                @CamControl.canceled -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnCamControl;
                @MachineGun.started -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnMachineGun;
                @MachineGun.performed -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnMachineGun;
                @MachineGun.canceled -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnMachineGun;
                @Missile.started -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnMissile;
                @Missile.performed -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnMissile;
                @Missile.canceled -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnMissile;
                @MenuAction.started -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnMenuAction;
                @MenuAction.performed -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnMenuAction;
                @MenuAction.canceled -= m_Wrapper.m_HeliControllerActionsCallbackInterface.OnMenuAction;
            }
            m_Wrapper.m_HeliControllerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Acc.started += instance.OnAcc;
                @Acc.performed += instance.OnAcc;
                @Acc.canceled += instance.OnAcc;
                @Dcc.started += instance.OnDcc;
                @Dcc.performed += instance.OnDcc;
                @Dcc.canceled += instance.OnDcc;
                @Tilt.started += instance.OnTilt;
                @Tilt.performed += instance.OnTilt;
                @Tilt.canceled += instance.OnTilt;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @Unlock.started += instance.OnUnlock;
                @Unlock.performed += instance.OnUnlock;
                @Unlock.canceled += instance.OnUnlock;
                @CamControl.started += instance.OnCamControl;
                @CamControl.performed += instance.OnCamControl;
                @CamControl.canceled += instance.OnCamControl;
                @MachineGun.started += instance.OnMachineGun;
                @MachineGun.performed += instance.OnMachineGun;
                @MachineGun.canceled += instance.OnMachineGun;
                @Missile.started += instance.OnMissile;
                @Missile.performed += instance.OnMissile;
                @Missile.canceled += instance.OnMissile;
                @MenuAction.started += instance.OnMenuAction;
                @MenuAction.performed += instance.OnMenuAction;
                @MenuAction.canceled += instance.OnMenuAction;
            }
        }
    }
    public HeliControllerActions @HeliController => new HeliControllerActions(this);
    public interface IHeliControllerActions
    {
        void OnAcc(InputAction.CallbackContext context);
        void OnDcc(InputAction.CallbackContext context);
        void OnTilt(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
        void OnUnlock(InputAction.CallbackContext context);
        void OnCamControl(InputAction.CallbackContext context);
        void OnMachineGun(InputAction.CallbackContext context);
        void OnMissile(InputAction.CallbackContext context);
        void OnMenuAction(InputAction.CallbackContext context);
    }
}
