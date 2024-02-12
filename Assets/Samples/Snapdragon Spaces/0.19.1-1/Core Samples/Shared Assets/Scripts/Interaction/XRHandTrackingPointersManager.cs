/******************************************************************************
 * File: XRHandTrackingPointersManager.cs
 * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

#if QCHT_UNITY_CORE
using QCHT.Interactions.Core;
using QCHT.Interactions.Distal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class XRHandTrackingPointersManager : MonoBehaviour
    {
        [SerializeField] [Tooltip("Input Action for checking if the left hand is being tracked.")]
        private InputAction leftTrackingState;

        [SerializeField] [Tooltip("Input Action for checking if the right hand is being tracked.")]
        private InputAction rightTrackingState;

        [SerializeField] [Tooltip("Input Action for checking the flip ratio on the left hand")]
        private InputAction leftFlipRatio;

        [SerializeField] [Tooltip("Input Action for checking the right ratio on the left hand")]
        private InputAction rightFlipRatio;

        [SerializeField] [Tooltip("Reference to the XRController Game Object used for the left hand.")]
        private GameObject leftHandXRController;

        [SerializeField] [Tooltip("Reference to the XRController Game Object used for the right hand.")]
        private GameObject rightHandXRController;

#if QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
        [SerializeField] [Tooltip("Reference to the XR UI Input Module component in the scene.")]
        private XRUIInputModule _xrUIInputModule;

        private InteractionManager _interactionManager;
        private EventSystem _eventSystem;
#endif

        private float _timer;

        private void Awake()
        {
            _interactionManager = InteractionManager.Instance;
            _eventSystem = EventSystem.current;
        }

        private void Start()
        {
#if QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
            ConfigureHandRays(leftHandXRController, true);
            ConfigureHandRays(rightHandXRController, false);
#endif
        }

        private void Update()
        {
            leftHandXRController.SetActive(leftTrackingState.IsInProgress() && leftFlipRatio.ReadValue<float>() <= 0f);
            rightHandXRController.SetActive(rightTrackingState.IsInProgress() && rightFlipRatio.ReadValue<float>() <= 0f);
#if QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
            if (_interactionManager.HandTrackingManager == null)
            {
                return;
            }

            // Check if the Hand Tracking Status is not error, otherwise after 0.5 seconds it will switch to the next input method.
            if (_interactionManager.HandTrackingManager.HandTrackingStatus != HandTrackingStatus.Error)
            {
                _timer = 0f;
                return;
            }

            _timer += Time.deltaTime;
            if (_timer > 0.5f)
            {
                _timer = 0f;
                _interactionManager.SwitchInput();
            }

#endif
#if !QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
            if (leftTrackingState.IsInProgress() || rightTrackingState.IsInProgress())
            {
                _timer = 0f;
            }
            else
            {
                if (_timer > 10f)
                {
                    _timer = 0f;
                    _interactionManager.SwitchInput();
                }

                _timer += Time.deltaTime;
            }
#endif
        }

        private void OnEnable()
        {
            leftTrackingState.Enable();
            rightTrackingState.Enable();
            leftFlipRatio.Enable();
            rightFlipRatio.Enable();
#if QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
            HandleComponents(false);
#endif
        }

        private void OnDisable()
        {
            leftTrackingState.Disable();
            rightTrackingState.Disable();
            leftFlipRatio.Disable();
            rightFlipRatio.Disable();
#if QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
            HandleComponents(true);
#endif
        }

        private void HandleComponents(bool enable)
        {
            _xrUIInputModule.enabled = enable;
            _eventSystem.enabled = enable;
        }

        private void ConfigureHandRays(GameObject RayVisualization, bool isLeft)
        {
            // Check if there is a XRInteractorLineVisual component.
            var legacyRay = RayVisualization.GetComponent<XRInteractorLineVisual>();
            if (legacyRay == null)
            {
                return;
            }

            // Add a xrRayToUIInteractor component for adjusting XRIT Ray to match the visual correction line.
            var xrRayToUIInteractor = RayVisualization.AddComponent<XRRayToUIInteractor>();
            xrRayToUIInteractor.IsLeft = isLeft;
            // Create a new XRRayInteractorLineVisual and pass the configuration of the XRInteractorLineVisual.
            var newRay = RayVisualization.AddComponent<XRRayInteractorLineVisual>();
            newRay.enabled = false;
            newRay.LineWidthMultiplier = legacyRay.lineWidth;
            newRay.SelectColorGradient = legacyRay.validColorGradient;
            newRay.IdleColorGradient = legacyRay.invalidColorGradient;
            legacyRay.enabled = false;
            newRay.enabled = true;
        }
    }
}
#endif
