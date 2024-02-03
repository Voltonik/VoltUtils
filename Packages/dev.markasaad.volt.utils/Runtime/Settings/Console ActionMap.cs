//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Packages/dev.markasaad.volt.utils/Runtime/Settings/Console ActionMap.inputactions
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

public partial class @ConsoleActionMap: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @ConsoleActionMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Console ActionMap"",
    ""maps"": [
        {
            ""name"": ""Console"",
            ""id"": ""98bd1fc2-32f3-48d3-bf3e-95fa141e93c7"",
            ""actions"": [
                {
                    ""name"": ""Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""f0e40b0a-8077-4c8b-980a-746fa1371f5b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AutoComplete"",
                    ""type"": ""Button"",
                    ""id"": ""9d55dfaa-b186-406f-8739-9641f0d19279"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""History Up"",
                    ""type"": ""Button"",
                    ""id"": ""10188c27-5779-4a64-90cc-53e377749d7f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""History Down"",
                    ""type"": ""Button"",
                    ""id"": ""525aa962-d1c5-4d33-b8f6-452bca4152e3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5c38ebfb-1b4f-49ed-a20c-cd608115f83c"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""587693cf-6399-41a3-8a97-0fa8ca375a0c"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AutoComplete"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e0a15e09-b929-4820-adf0-d36ece4b5870"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""History Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""76733434-8241-4776-975e-cb76d58d8be4"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""History Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Console
        m_Console = asset.FindActionMap("Console", throwIfNotFound: true);
        m_Console_Toggle = m_Console.FindAction("Toggle", throwIfNotFound: true);
        m_Console_AutoComplete = m_Console.FindAction("AutoComplete", throwIfNotFound: true);
        m_Console_HistoryUp = m_Console.FindAction("History Up", throwIfNotFound: true);
        m_Console_HistoryDown = m_Console.FindAction("History Down", throwIfNotFound: true);
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

    // Console
    private readonly InputActionMap m_Console;
    private List<IConsoleActions> m_ConsoleActionsCallbackInterfaces = new List<IConsoleActions>();
    private readonly InputAction m_Console_Toggle;
    private readonly InputAction m_Console_AutoComplete;
    private readonly InputAction m_Console_HistoryUp;
    private readonly InputAction m_Console_HistoryDown;
    public struct ConsoleActions
    {
        private @ConsoleActionMap m_Wrapper;
        public ConsoleActions(@ConsoleActionMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Toggle => m_Wrapper.m_Console_Toggle;
        public InputAction @AutoComplete => m_Wrapper.m_Console_AutoComplete;
        public InputAction @HistoryUp => m_Wrapper.m_Console_HistoryUp;
        public InputAction @HistoryDown => m_Wrapper.m_Console_HistoryDown;
        public InputActionMap Get() { return m_Wrapper.m_Console; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ConsoleActions set) { return set.Get(); }
        public void AddCallbacks(IConsoleActions instance)
        {
            if (instance == null || m_Wrapper.m_ConsoleActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_ConsoleActionsCallbackInterfaces.Add(instance);
            @Toggle.started += instance.OnToggle;
            @Toggle.performed += instance.OnToggle;
            @Toggle.canceled += instance.OnToggle;
            @AutoComplete.started += instance.OnAutoComplete;
            @AutoComplete.performed += instance.OnAutoComplete;
            @AutoComplete.canceled += instance.OnAutoComplete;
            @HistoryUp.started += instance.OnHistoryUp;
            @HistoryUp.performed += instance.OnHistoryUp;
            @HistoryUp.canceled += instance.OnHistoryUp;
            @HistoryDown.started += instance.OnHistoryDown;
            @HistoryDown.performed += instance.OnHistoryDown;
            @HistoryDown.canceled += instance.OnHistoryDown;
        }

        private void UnregisterCallbacks(IConsoleActions instance)
        {
            @Toggle.started -= instance.OnToggle;
            @Toggle.performed -= instance.OnToggle;
            @Toggle.canceled -= instance.OnToggle;
            @AutoComplete.started -= instance.OnAutoComplete;
            @AutoComplete.performed -= instance.OnAutoComplete;
            @AutoComplete.canceled -= instance.OnAutoComplete;
            @HistoryUp.started -= instance.OnHistoryUp;
            @HistoryUp.performed -= instance.OnHistoryUp;
            @HistoryUp.canceled -= instance.OnHistoryUp;
            @HistoryDown.started -= instance.OnHistoryDown;
            @HistoryDown.performed -= instance.OnHistoryDown;
            @HistoryDown.canceled -= instance.OnHistoryDown;
        }

        public void RemoveCallbacks(IConsoleActions instance)
        {
            if (m_Wrapper.m_ConsoleActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IConsoleActions instance)
        {
            foreach (var item in m_Wrapper.m_ConsoleActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_ConsoleActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public ConsoleActions @Console => new ConsoleActions(this);
    public interface IConsoleActions
    {
        void OnToggle(InputAction.CallbackContext context);
        void OnAutoComplete(InputAction.CallbackContext context);
        void OnHistoryUp(InputAction.CallbackContext context);
        void OnHistoryDown(InputAction.CallbackContext context);
    }
}
