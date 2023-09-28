/******************************************************************************
 * File: HandTrackingUISetup.cs
 * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class HandTrackingUISetup : MonoBehaviour
    {
        [Tooltip("Collider helper to work with XR Ray Interactor Line Visual")]
        public BoxCollider GeneralUICollider;

        private void Start()
        {
            // Get the rect size of the UI to adapt the collider.
            var rectComponent = gameObject.GetComponent<RectTransform>().rect;
            GeneralUICollider.size = new Vector3(rectComponent.width, rectComponent.height, 0.1f);
        }

        private void OnEnable()
        {
            InteractionManager.onInputTypeSwitch += SetupUIColliders;
        }

        private void OnDisable()
        {
            InteractionManager.onInputTypeSwitch -= SetupUIColliders;
        }

        // Subscribe to the InputTypeSwitch from the Interaction Manager and only enable the collider when using Hand Tracking as the input system.
        private void SetupUIColliders(InputType InputType)
        {
            GeneralUICollider.enabled = InputType == InputType.HandTracking;
        }
    }
}
