/******************************************************************************
 * File: XRInteractorButtonBinding.cs
 * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class SpacesHandTrackingButtonBinding : MonoBehaviour
    {
        [Tooltip("Reference to the XR Simple Interactable if there is one on the Game Object.")]
        public XRSimpleInteractable XrSimpleInteractable;

        [Tooltip("Reference to the Snapping volume Game Object if there is one for this component.")]
        public GameObject SnappingVolumeGameObject;

        private void OnEnable()
        {
            if (InteractionManager.Instance != null)
            {
                HandleInputSwitch(InteractionManager.Instance.InputType);
            }
            InteractionManager.onInputTypeSwitch += HandleInputSwitch;
        }

        private void OnDisable()
        {
            InteractionManager.onInputTypeSwitch -= HandleInputSwitch;
        }

        private void HandleInputSwitch(InputType InputType)
        {
            if (XrSimpleInteractable != null)
            {
                XrSimpleInteractable.enabled = InputType == InputType.HandTracking;
            }

            if (SnappingVolumeGameObject != null)
            {
                SnappingVolumeGameObject.SetActive(InputType == InputType.HandTracking);
            }
        }
    }
}
