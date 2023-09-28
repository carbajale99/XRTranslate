/******************************************************************************
 * File: SampleController.cs
 * Copyright (c) 2021-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR;
#if AR_FOUNDATION_5_0_OR_NEWER
using Unity.XR.CoreUtils;
#endif

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class SampleController : MonoBehaviour
    {
        public delegate void PrimaryButtonPressed();

        public InteractionManager InteractionManager;
        public InputActionReference LeftControllerPrimary;
        public InputActionReference RightControllerPrimary;
        public bool RunSubsystemChecks = true;
        public List<GameObject> ContentOnPassed;
        public List<GameObject> ContentOnFailed;
        protected static PrimaryButtonPressed _primaryButtonPressed;
        protected bool SubsystemChecksPassed;
        protected BaseRuntimeFeature _baseRuntimeFeature { get; private set; }
        protected bool _isPassthroughOn { get; private set; }

        public virtual void Start()
        {
            foreach (var content in ContentOnPassed)
            {
                content.SetActive(SubsystemChecksPassed);
            }

            foreach (var content in ContentOnFailed)
            {
                content.SetActive(!SubsystemChecksPassed);
            }

            if (!SubsystemChecksPassed)
            {
                Debug.LogWarning("Subsystem checks failed. Some features may be unavailable.");
            }

            if (!_baseRuntimeFeature)
            {
                Debug.LogWarning("Base Runtime Feature isn't available.");
                return;
            }

            if (!_baseRuntimeFeature.IsPassthroughSupported())
            {
                return;
            }

            _isPassthroughOn = _baseRuntimeFeature.GetPassthroughEnabled();
            _baseRuntimeFeature.SetPassthroughEnabled(_isPassthroughOn);
        }

        public void Quit()
        {
            SendHapticImpulse();
            Application.Quit();
        }

        public void SendHapticImpulse(float amplitude = 0.5f, float frequency = 60f, float duration = 0.1f)
        {
            InteractionManager.SendHapticImpulse(amplitude, frequency, duration);
        }

        public virtual void OnEnable()
        {
            LeftControllerPrimary.action.performed += OnPrimaryButtonPressed;
            RightControllerPrimary.action.performed += OnPrimaryButtonPressed;
            _baseRuntimeFeature = OpenXRSettings.Instance.GetFeature<BaseRuntimeFeature>();
            SubsystemChecksPassed = _baseRuntimeFeature != null && GetSubsystemCheck();
        }

        public virtual void OnDisable()
        {
            LeftControllerPrimary.action.performed -= OnPrimaryButtonPressed;
            RightControllerPrimary.action.performed -= OnPrimaryButtonPressed;
        }

        public void TogglePassthroughWithCheckbox()
        {
            TogglePassthrough();
        }

        protected bool GetSubsystemCheck()
        {
            return !RunSubsystemChecks || CheckSubsystem();
        }

        protected virtual bool CheckSubsystem()
        {
            Debug.LogWarning("No subsystem check was performed. Derived classes from SampleController must implement their own check.");
            return false;
        }

        private void OnPrimaryButtonPressed(InputAction.CallbackContext ctx)
        {
            if (!_baseRuntimeFeature.IsPassthroughSupported() && InteractionManager.InputType == InputType.HandTracking)
            {
                return;
            }

            TogglePassthrough();
            _primaryButtonPressed?.Invoke();
        }

        private void TogglePassthrough()
        {
            _isPassthroughOn = !_isPassthroughOn;
            _baseRuntimeFeature.SetPassthroughEnabled(_isPassthroughOn);
        }
    }
}
