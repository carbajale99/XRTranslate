/******************************************************************************
 * File: AutomaticControllerSwitch.cs
 * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/
#if QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
using QCHT.Interactions.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class AutomaticControllerSwitch : MonoBehaviour
    {
        [SerializeField] [Tooltip("Reference to the scene Interaction Manager")]
        private InteractionManager InteractionManager;

        [SerializeField] [Tooltip("Reference to the left controller isTracked input action property")]
        private InputActionProperty _leftControllerTracked;

        [SerializeField] [Tooltip("Reference to the right controller isTracked input action property")]
        private InputActionProperty _rightControllerTracked;

        private bool _controllerProfileSet;
        private InputType _inputType;
        private XRControllerProfile _xrControllerProfile;
        public bool ControllersTracked { get; private set; }

        private void Update()
        {
            CheckXRControllersState();
        }

        private void OnEnable()
        {
            _leftControllerTracked.EnableDirectAction();
            _rightControllerTracked.EnableDirectAction();
        }

        private void OnDisable()
        {
            _leftControllerTracked.DisableDirectAction();
            _rightControllerTracked.DisableDirectAction();
        }

        /// <summary>
        ///     Setup the controller profile
        /// </summary>
        /// <param name="NewXrControllerProfile"></param>
        public void SetControllerProfile(XRControllerProfile NewXrControllerProfile)
        {
            _xrControllerProfile = NewXrControllerProfile;
            _controllerProfileSet = true;
        }

        // Checks whether if the XR Controllers are tracked to turn off Hand Tracking and vice versa.
        private void CheckXRControllersState()
        {
            // If it is running on the host controller or the controller profile is not set don't continue.
            if (_xrControllerProfile == XRControllerProfile.HostController || !_controllerProfileSet || InteractionManager.HandTrackingManager == null)
            {
                return;
            }

            // Set if any controller is tracked.
            ControllersTracked = IsControllerTracked(_leftControllerTracked) || IsControllerTracked(_rightControllerTracked);

            // Stop Hand Tracking if controllers are tracked, regardless of current InputType
            if (ControllersTracked)
            {
                InteractionManager.HandTrackingManager.StopHandTracking();
            }

            // Do not switch input if Gaze Pointer currently selected
            if (InteractionManager.InputType == InputType.GazePointer)
            {
                return;
            }

            // Switch between Hand Tracking or Controller Pointer depending on the device connection.
            if (ControllersTracked)
            {
                SwitchToControllerInput();
            }
            else
            {
                SwitchToHandTrackingInput();
            }
        }

        // Switch to Controller Pointer Input Type and stop Hand Tracking.
        private void SwitchToControllerInput()
        {
            if (InteractionManager.InputType == InputType.ControllerPointer)
            {
                return;
            }

            InteractionManager.SwitchInput(InputType.ControllerPointer);

        }

        // Switch to Hand Tracking Input Type.
        private void SwitchToHandTrackingInput()
        {
            if (InteractionManager.InputType == InputType.HandTracking)
            {
                return;
            }

            InteractionManager.SwitchInput(InputType.HandTracking);
        }

        private bool IsControllerTracked(InputActionProperty controllerTracked)
        {
            return controllerTracked.action.ReadValue<float>() > 0;
        }
    }
}
#endif
