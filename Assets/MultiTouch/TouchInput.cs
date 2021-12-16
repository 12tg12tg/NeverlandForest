//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.1.1
//     from Assets/TouchInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @TouchInput : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @TouchInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""TouchInput"",
    ""maps"": [
        {
            ""name"": ""Touchs"",
            ""id"": ""32189e44-8553-4d03-83ed-f24083a8d3f3"",
            ""actions"": [
                {
                    ""name"": ""PrimaryContactForTap"",
                    ""type"": ""Button"",
                    ""id"": ""3beddb0c-99a7-492c-bdd1-746d5d293be5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""MultiTap(tapDelay=0.25),Tap,SlowTap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PrimaryPostionForSwipe"",
                    ""type"": ""Value"",
                    ""id"": ""cffa4e51-c291-4c60-98bd-edd2b9e0655e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""PrimaryContactForSwipe"",
                    ""type"": ""Button"",
                    ""id"": ""5e0f5927-2fb6-4b18-b466-d430bf8827d9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TouchPosition1"",
                    ""type"": ""PassThrough"",
                    ""id"": ""339238cd-64bd-48c4-be2d-63b517b8e157"",
                    ""expectedControlType"": ""Touch"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TouchPosition2"",
                    ""type"": ""PassThrough"",
                    ""id"": ""458da40a-5d5b-41a2-8dcf-163c847c8681"",
                    ""expectedControlType"": ""Touch"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d2a8d674-f4ba-4d8e-9f8d-15e8ba14ab1e"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PrimaryContactForTap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc42d064-8f93-497a-8c34-5c417215df16"",
                    ""path"": ""<Touchscreen>/touch0"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchPosition1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6fab4d85-3da6-4019-b791-8d13abf35fc1"",
                    ""path"": ""<Touchscreen>/touch1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchPosition2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1fad49b5-def9-4080-bd3a-c65ab8efc960"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PrimaryPostionForSwipe"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d73236a1-fd31-4fdf-b269-4bc86c31797c"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PrimaryContactForSwipe"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Touchs
        m_Touchs = asset.FindActionMap("Touchs", throwIfNotFound: true);
        m_Touchs_PrimaryContactForTap = m_Touchs.FindAction("PrimaryContactForTap", throwIfNotFound: true);
        m_Touchs_PrimaryPostionForSwipe = m_Touchs.FindAction("PrimaryPostionForSwipe", throwIfNotFound: true);
        m_Touchs_PrimaryContactForSwipe = m_Touchs.FindAction("PrimaryContactForSwipe", throwIfNotFound: true);
        m_Touchs_TouchPosition1 = m_Touchs.FindAction("TouchPosition1", throwIfNotFound: true);
        m_Touchs_TouchPosition2 = m_Touchs.FindAction("TouchPosition2", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Touchs
    private readonly InputActionMap m_Touchs;
    private ITouchsActions m_TouchsActionsCallbackInterface;
    private readonly InputAction m_Touchs_PrimaryContactForTap;
    private readonly InputAction m_Touchs_PrimaryPostionForSwipe;
    private readonly InputAction m_Touchs_PrimaryContactForSwipe;
    private readonly InputAction m_Touchs_TouchPosition1;
    private readonly InputAction m_Touchs_TouchPosition2;
    public struct TouchsActions
    {
        private @TouchInput m_Wrapper;
        public TouchsActions(@TouchInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @PrimaryContactForTap => m_Wrapper.m_Touchs_PrimaryContactForTap;
        public InputAction @PrimaryPostionForSwipe => m_Wrapper.m_Touchs_PrimaryPostionForSwipe;
        public InputAction @PrimaryContactForSwipe => m_Wrapper.m_Touchs_PrimaryContactForSwipe;
        public InputAction @TouchPosition1 => m_Wrapper.m_Touchs_TouchPosition1;
        public InputAction @TouchPosition2 => m_Wrapper.m_Touchs_TouchPosition2;
        public InputActionMap Get() { return m_Wrapper.m_Touchs; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TouchsActions set) { return set.Get(); }
        public void SetCallbacks(ITouchsActions instance)
        {
            if (m_Wrapper.m_TouchsActionsCallbackInterface != null)
            {
                @PrimaryContactForTap.started -= m_Wrapper.m_TouchsActionsCallbackInterface.OnPrimaryContactForTap;
                @PrimaryContactForTap.performed -= m_Wrapper.m_TouchsActionsCallbackInterface.OnPrimaryContactForTap;
                @PrimaryContactForTap.canceled -= m_Wrapper.m_TouchsActionsCallbackInterface.OnPrimaryContactForTap;
                @PrimaryPostionForSwipe.started -= m_Wrapper.m_TouchsActionsCallbackInterface.OnPrimaryPostionForSwipe;
                @PrimaryPostionForSwipe.performed -= m_Wrapper.m_TouchsActionsCallbackInterface.OnPrimaryPostionForSwipe;
                @PrimaryPostionForSwipe.canceled -= m_Wrapper.m_TouchsActionsCallbackInterface.OnPrimaryPostionForSwipe;
                @PrimaryContactForSwipe.started -= m_Wrapper.m_TouchsActionsCallbackInterface.OnPrimaryContactForSwipe;
                @PrimaryContactForSwipe.performed -= m_Wrapper.m_TouchsActionsCallbackInterface.OnPrimaryContactForSwipe;
                @PrimaryContactForSwipe.canceled -= m_Wrapper.m_TouchsActionsCallbackInterface.OnPrimaryContactForSwipe;
                @TouchPosition1.started -= m_Wrapper.m_TouchsActionsCallbackInterface.OnTouchPosition1;
                @TouchPosition1.performed -= m_Wrapper.m_TouchsActionsCallbackInterface.OnTouchPosition1;
                @TouchPosition1.canceled -= m_Wrapper.m_TouchsActionsCallbackInterface.OnTouchPosition1;
                @TouchPosition2.started -= m_Wrapper.m_TouchsActionsCallbackInterface.OnTouchPosition2;
                @TouchPosition2.performed -= m_Wrapper.m_TouchsActionsCallbackInterface.OnTouchPosition2;
                @TouchPosition2.canceled -= m_Wrapper.m_TouchsActionsCallbackInterface.OnTouchPosition2;
            }
            m_Wrapper.m_TouchsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PrimaryContactForTap.started += instance.OnPrimaryContactForTap;
                @PrimaryContactForTap.performed += instance.OnPrimaryContactForTap;
                @PrimaryContactForTap.canceled += instance.OnPrimaryContactForTap;
                @PrimaryPostionForSwipe.started += instance.OnPrimaryPostionForSwipe;
                @PrimaryPostionForSwipe.performed += instance.OnPrimaryPostionForSwipe;
                @PrimaryPostionForSwipe.canceled += instance.OnPrimaryPostionForSwipe;
                @PrimaryContactForSwipe.started += instance.OnPrimaryContactForSwipe;
                @PrimaryContactForSwipe.performed += instance.OnPrimaryContactForSwipe;
                @PrimaryContactForSwipe.canceled += instance.OnPrimaryContactForSwipe;
                @TouchPosition1.started += instance.OnTouchPosition1;
                @TouchPosition1.performed += instance.OnTouchPosition1;
                @TouchPosition1.canceled += instance.OnTouchPosition1;
                @TouchPosition2.started += instance.OnTouchPosition2;
                @TouchPosition2.performed += instance.OnTouchPosition2;
                @TouchPosition2.canceled += instance.OnTouchPosition2;
            }
        }
    }
    public TouchsActions @Touchs => new TouchsActions(this);
    public interface ITouchsActions
    {
        void OnPrimaryContactForTap(InputAction.CallbackContext context);
        void OnPrimaryPostionForSwipe(InputAction.CallbackContext context);
        void OnPrimaryContactForSwipe(InputAction.CallbackContext context);
        void OnTouchPosition1(InputAction.CallbackContext context);
        void OnTouchPosition2(InputAction.CallbackContext context);
    }
}
