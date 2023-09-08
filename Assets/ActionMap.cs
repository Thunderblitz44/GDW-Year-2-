//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.1
//     from Assets/ActionMap.inputactions
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

public partial class @ActionMap: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @ActionMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ActionMap"",
    ""maps"": [
        {
            ""name"": ""Locomotion"",
            ""id"": ""feade954-53aa-4280-bd93-deb5f5dcb271"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""0d18076a-39a1-4a51-8b56-1d5ff0dd205f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""a1ab0040-44bf-4f2a-8694-ee90bf96f728"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""5d887ffa-9bc1-4739-9b49-5b5c39bff830"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""2d7b5492-034c-45b7-af29-0995ffdd1a4b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""d6ed79fd-7a96-4f3c-886e-fb77439921c4"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""78ecafad-f199-4fe2-8e8f-dd15346edac0"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d9ffd16e-a875-4c34-a717-1952b9a2e889"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2e4b7e3c-76b6-4179-b758-0329d58d9e2b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5c3a5a8c-e062-44a4-8ad1-efd8dbf54989"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""f476e75b-82d9-4cd0-910f-b312738b25b7"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3c28fe85-dd49-48e8-a353-755a934a9d71"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""efa7e79b-2183-423d-8c3f-e6a60b5def2c"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1031866c-8c3c-4eaf-afb3-2e84380f95a5"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""07b1158b-96e7-4b3a-a980-9e916c3956c4"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0bf05cd3-30fa-412f-af6d-b576eb73abef"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aabd15af-5bc4-40c7-a4f1-91c638b2c693"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""88b9502e-aacf-4233-ba29-0e78ddde7499"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0d25a1db-91d8-45f0-9f93-b323349510a6"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e25b2a87-583a-42de-9deb-5e744963b136"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""74e74577-f2a0-48b8-b367-5ae72e4c2325"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""CameraControl"",
            ""id"": ""20b2cb7d-b37e-4108-b0cf-c554537bedbb"",
            ""actions"": [
                {
                    ""name"": ""LockOnToTarget"",
                    ""type"": ""Button"",
                    ""id"": ""cfc2f0d9-3def-40e4-a3f6-bdbe7f7d897d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CycleTargets"",
                    ""type"": ""Button"",
                    ""id"": ""de9a90db-890e-4372-ace2-f5dcf5ebd80a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""a555938b-472a-4676-ba43-13c09c6ac675"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""b10125f1-20e3-4220-a923-7d33c2ce964c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""686dea2d-f0aa-4914-9746-a5ee940eef32"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LockOnToTarget"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""17c97f84-1a89-4470-81af-a8433880dc16"",
                    ""path"": ""<Gamepad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LockOnToTarget"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""61df7e55-88c9-4b5a-b7ab-5655f9eabae2"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CycleTargets"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""38bd829f-495e-4ee5-9de3-a984cf342fed"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9d256245-29d9-499a-ada8-ffc3e8b5f608"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""301244f1-97a9-45a0-8e66-77f9b0c13fda"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""370129bb-e467-46b6-88c9-d0a78e5ac71d"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d5e3a90a-0743-47a1-a541-db07d95a4ebf"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d8ea3fa7-85e0-4ff0-afe7-4d9aad9bc6a8"",
                    ""path"": ""<Gamepad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6e8fbe84-e5cb-436b-847a-4c63eca94e5e"",
                    ""path"": ""<Gamepad>/rightStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Abilities"",
            ""id"": ""67f28106-a61a-4a94-b855-a47b15a52e64"",
            ""actions"": [
                {
                    ""name"": ""GrappleAbility"",
                    ""type"": ""Button"",
                    ""id"": ""9f3c807d-a6e5-41e5-8568-d33f36b15a62"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""BuffAbility"",
                    ""type"": ""Button"",
                    ""id"": ""9e3b9161-d97c-4366-b296-04fd4d26f2d6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DashAbility"",
                    ""type"": ""Button"",
                    ""id"": ""11389b02-2b13-4f7a-9544-27ac2f94c6c1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ShieldAbility"",
                    ""type"": ""Button"",
                    ""id"": ""4530299a-12f2-40b7-87c4-6ab00f1e5ab4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5939ac66-746e-4701-bed1-c13332e1a31a"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GrappleAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c9616c1-040f-4854-a2bc-721a209036d5"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GrappleAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c33b34ae-2373-4764-81dd-65789730013b"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BuffAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""58266a59-bcc5-49cb-a5e6-41085c917ccb"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BuffAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""699b1234-178f-46a8-8c0d-e59015358438"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DashAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""73492789-3dea-4701-b5ca-b1ba1cb52d01"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DashAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a3125358-9589-4001-a440-3acfa3e002e6"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShieldAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce43c31a-fbeb-47fd-8446-a83840c9100d"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShieldAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""General"",
            ""id"": ""78997225-4526-4f61-bdfc-a712a3ec25f6"",
            ""actions"": [
                {
                    ""name"": ""Escape"",
                    ""type"": ""Button"",
                    ""id"": ""f14356dc-7924-4465-a04a-320b0ff48c5e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e46696d6-a74c-4504-a4e7-f82204d4a25e"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Locomotion
        m_Locomotion = asset.FindActionMap("Locomotion", throwIfNotFound: true);
        m_Locomotion_Move = m_Locomotion.FindAction("Move", throwIfNotFound: true);
        m_Locomotion_Jump = m_Locomotion.FindAction("Jump", throwIfNotFound: true);
        m_Locomotion_Run = m_Locomotion.FindAction("Run", throwIfNotFound: true);
        m_Locomotion_Crouch = m_Locomotion.FindAction("Crouch", throwIfNotFound: true);
        // CameraControl
        m_CameraControl = asset.FindActionMap("CameraControl", throwIfNotFound: true);
        m_CameraControl_LockOnToTarget = m_CameraControl.FindAction("LockOnToTarget", throwIfNotFound: true);
        m_CameraControl_CycleTargets = m_CameraControl.FindAction("CycleTargets", throwIfNotFound: true);
        m_CameraControl_Aim = m_CameraControl.FindAction("Aim", throwIfNotFound: true);
        m_CameraControl_Look = m_CameraControl.FindAction("Look", throwIfNotFound: true);
        // Abilities
        m_Abilities = asset.FindActionMap("Abilities", throwIfNotFound: true);
        m_Abilities_GrappleAbility = m_Abilities.FindAction("GrappleAbility", throwIfNotFound: true);
        m_Abilities_BuffAbility = m_Abilities.FindAction("BuffAbility", throwIfNotFound: true);
        m_Abilities_DashAbility = m_Abilities.FindAction("DashAbility", throwIfNotFound: true);
        m_Abilities_ShieldAbility = m_Abilities.FindAction("ShieldAbility", throwIfNotFound: true);
        // General
        m_General = asset.FindActionMap("General", throwIfNotFound: true);
        m_General_Escape = m_General.FindAction("Escape", throwIfNotFound: true);
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

    // Locomotion
    private readonly InputActionMap m_Locomotion;
    private List<ILocomotionActions> m_LocomotionActionsCallbackInterfaces = new List<ILocomotionActions>();
    private readonly InputAction m_Locomotion_Move;
    private readonly InputAction m_Locomotion_Jump;
    private readonly InputAction m_Locomotion_Run;
    private readonly InputAction m_Locomotion_Crouch;
    public struct LocomotionActions
    {
        private @ActionMap m_Wrapper;
        public LocomotionActions(@ActionMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Locomotion_Move;
        public InputAction @Jump => m_Wrapper.m_Locomotion_Jump;
        public InputAction @Run => m_Wrapper.m_Locomotion_Run;
        public InputAction @Crouch => m_Wrapper.m_Locomotion_Crouch;
        public InputActionMap Get() { return m_Wrapper.m_Locomotion; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(LocomotionActions set) { return set.Get(); }
        public void AddCallbacks(ILocomotionActions instance)
        {
            if (instance == null || m_Wrapper.m_LocomotionActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_LocomotionActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Run.started += instance.OnRun;
            @Run.performed += instance.OnRun;
            @Run.canceled += instance.OnRun;
            @Crouch.started += instance.OnCrouch;
            @Crouch.performed += instance.OnCrouch;
            @Crouch.canceled += instance.OnCrouch;
        }

        private void UnregisterCallbacks(ILocomotionActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Run.started -= instance.OnRun;
            @Run.performed -= instance.OnRun;
            @Run.canceled -= instance.OnRun;
            @Crouch.started -= instance.OnCrouch;
            @Crouch.performed -= instance.OnCrouch;
            @Crouch.canceled -= instance.OnCrouch;
        }

        public void RemoveCallbacks(ILocomotionActions instance)
        {
            if (m_Wrapper.m_LocomotionActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ILocomotionActions instance)
        {
            foreach (var item in m_Wrapper.m_LocomotionActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_LocomotionActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public LocomotionActions @Locomotion => new LocomotionActions(this);

    // CameraControl
    private readonly InputActionMap m_CameraControl;
    private List<ICameraControlActions> m_CameraControlActionsCallbackInterfaces = new List<ICameraControlActions>();
    private readonly InputAction m_CameraControl_LockOnToTarget;
    private readonly InputAction m_CameraControl_CycleTargets;
    private readonly InputAction m_CameraControl_Aim;
    private readonly InputAction m_CameraControl_Look;
    public struct CameraControlActions
    {
        private @ActionMap m_Wrapper;
        public CameraControlActions(@ActionMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @LockOnToTarget => m_Wrapper.m_CameraControl_LockOnToTarget;
        public InputAction @CycleTargets => m_Wrapper.m_CameraControl_CycleTargets;
        public InputAction @Aim => m_Wrapper.m_CameraControl_Aim;
        public InputAction @Look => m_Wrapper.m_CameraControl_Look;
        public InputActionMap Get() { return m_Wrapper.m_CameraControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraControlActions set) { return set.Get(); }
        public void AddCallbacks(ICameraControlActions instance)
        {
            if (instance == null || m_Wrapper.m_CameraControlActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CameraControlActionsCallbackInterfaces.Add(instance);
            @LockOnToTarget.started += instance.OnLockOnToTarget;
            @LockOnToTarget.performed += instance.OnLockOnToTarget;
            @LockOnToTarget.canceled += instance.OnLockOnToTarget;
            @CycleTargets.started += instance.OnCycleTargets;
            @CycleTargets.performed += instance.OnCycleTargets;
            @CycleTargets.canceled += instance.OnCycleTargets;
            @Aim.started += instance.OnAim;
            @Aim.performed += instance.OnAim;
            @Aim.canceled += instance.OnAim;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
        }

        private void UnregisterCallbacks(ICameraControlActions instance)
        {
            @LockOnToTarget.started -= instance.OnLockOnToTarget;
            @LockOnToTarget.performed -= instance.OnLockOnToTarget;
            @LockOnToTarget.canceled -= instance.OnLockOnToTarget;
            @CycleTargets.started -= instance.OnCycleTargets;
            @CycleTargets.performed -= instance.OnCycleTargets;
            @CycleTargets.canceled -= instance.OnCycleTargets;
            @Aim.started -= instance.OnAim;
            @Aim.performed -= instance.OnAim;
            @Aim.canceled -= instance.OnAim;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
        }

        public void RemoveCallbacks(ICameraControlActions instance)
        {
            if (m_Wrapper.m_CameraControlActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICameraControlActions instance)
        {
            foreach (var item in m_Wrapper.m_CameraControlActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CameraControlActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CameraControlActions @CameraControl => new CameraControlActions(this);

    // Abilities
    private readonly InputActionMap m_Abilities;
    private List<IAbilitiesActions> m_AbilitiesActionsCallbackInterfaces = new List<IAbilitiesActions>();
    private readonly InputAction m_Abilities_GrappleAbility;
    private readonly InputAction m_Abilities_BuffAbility;
    private readonly InputAction m_Abilities_DashAbility;
    private readonly InputAction m_Abilities_ShieldAbility;
    public struct AbilitiesActions
    {
        private @ActionMap m_Wrapper;
        public AbilitiesActions(@ActionMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @GrappleAbility => m_Wrapper.m_Abilities_GrappleAbility;
        public InputAction @BuffAbility => m_Wrapper.m_Abilities_BuffAbility;
        public InputAction @DashAbility => m_Wrapper.m_Abilities_DashAbility;
        public InputAction @ShieldAbility => m_Wrapper.m_Abilities_ShieldAbility;
        public InputActionMap Get() { return m_Wrapper.m_Abilities; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(AbilitiesActions set) { return set.Get(); }
        public void AddCallbacks(IAbilitiesActions instance)
        {
            if (instance == null || m_Wrapper.m_AbilitiesActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_AbilitiesActionsCallbackInterfaces.Add(instance);
            @GrappleAbility.started += instance.OnGrappleAbility;
            @GrappleAbility.performed += instance.OnGrappleAbility;
            @GrappleAbility.canceled += instance.OnGrappleAbility;
            @BuffAbility.started += instance.OnBuffAbility;
            @BuffAbility.performed += instance.OnBuffAbility;
            @BuffAbility.canceled += instance.OnBuffAbility;
            @DashAbility.started += instance.OnDashAbility;
            @DashAbility.performed += instance.OnDashAbility;
            @DashAbility.canceled += instance.OnDashAbility;
            @ShieldAbility.started += instance.OnShieldAbility;
            @ShieldAbility.performed += instance.OnShieldAbility;
            @ShieldAbility.canceled += instance.OnShieldAbility;
        }

        private void UnregisterCallbacks(IAbilitiesActions instance)
        {
            @GrappleAbility.started -= instance.OnGrappleAbility;
            @GrappleAbility.performed -= instance.OnGrappleAbility;
            @GrappleAbility.canceled -= instance.OnGrappleAbility;
            @BuffAbility.started -= instance.OnBuffAbility;
            @BuffAbility.performed -= instance.OnBuffAbility;
            @BuffAbility.canceled -= instance.OnBuffAbility;
            @DashAbility.started -= instance.OnDashAbility;
            @DashAbility.performed -= instance.OnDashAbility;
            @DashAbility.canceled -= instance.OnDashAbility;
            @ShieldAbility.started -= instance.OnShieldAbility;
            @ShieldAbility.performed -= instance.OnShieldAbility;
            @ShieldAbility.canceled -= instance.OnShieldAbility;
        }

        public void RemoveCallbacks(IAbilitiesActions instance)
        {
            if (m_Wrapper.m_AbilitiesActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IAbilitiesActions instance)
        {
            foreach (var item in m_Wrapper.m_AbilitiesActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_AbilitiesActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public AbilitiesActions @Abilities => new AbilitiesActions(this);

    // General
    private readonly InputActionMap m_General;
    private List<IGeneralActions> m_GeneralActionsCallbackInterfaces = new List<IGeneralActions>();
    private readonly InputAction m_General_Escape;
    public struct GeneralActions
    {
        private @ActionMap m_Wrapper;
        public GeneralActions(@ActionMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Escape => m_Wrapper.m_General_Escape;
        public InputActionMap Get() { return m_Wrapper.m_General; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GeneralActions set) { return set.Get(); }
        public void AddCallbacks(IGeneralActions instance)
        {
            if (instance == null || m_Wrapper.m_GeneralActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GeneralActionsCallbackInterfaces.Add(instance);
            @Escape.started += instance.OnEscape;
            @Escape.performed += instance.OnEscape;
            @Escape.canceled += instance.OnEscape;
        }

        private void UnregisterCallbacks(IGeneralActions instance)
        {
            @Escape.started -= instance.OnEscape;
            @Escape.performed -= instance.OnEscape;
            @Escape.canceled -= instance.OnEscape;
        }

        public void RemoveCallbacks(IGeneralActions instance)
        {
            if (m_Wrapper.m_GeneralActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGeneralActions instance)
        {
            foreach (var item in m_Wrapper.m_GeneralActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GeneralActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GeneralActions @General => new GeneralActions(this);
    public interface ILocomotionActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
    }
    public interface ICameraControlActions
    {
        void OnLockOnToTarget(InputAction.CallbackContext context);
        void OnCycleTargets(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
    }
    public interface IAbilitiesActions
    {
        void OnGrappleAbility(InputAction.CallbackContext context);
        void OnBuffAbility(InputAction.CallbackContext context);
        void OnDashAbility(InputAction.CallbackContext context);
        void OnShieldAbility(InputAction.CallbackContext context);
    }
    public interface IGeneralActions
    {
        void OnEscape(InputAction.CallbackContext context);
    }
}
