/******************************************************************************
 * File: GazeInteractionUI.cs
 * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    [RequireComponent(typeof(XRGazeInteractor))]
    public class GazeInteractionUI : MonoBehaviour
    {
        [SerializeField] [Tooltip("Reference to the XRGazeInteractor component from XR Interaction Toolkit.")]
        private XRGazeInteractor _xrGazeInteractor;

        [Tooltip("Distance of the Gaze reticle from the main camera.")]
        public float DefaultDistance = 1.0f;

        [Tooltip("Delay time for the gaze pointer loading initialization")]
        public float DelayGazeLoading = 0.5f;

        [Tooltip("Reference to the reticle Game Object.")]
        public Transform ReticleGameObject;

        [Tooltip("Reference to the outer ring that will have the fill effect.")]
        public Image ReticleOuterRing;

        [SerializeField] [Tooltip("Input Action Reference to the select action.")]
        private InputActionReference SelectAction;
        private IPointerClickHandler _activeClickHandler;
        private float _gazeTimerCurrent;
        private bool _isSelectPressed;
        private float _safeTimerCurrent;
        public bool IsHovering { get; private set; }

        private void Update()
        {
            UpdateGazeCounter();
        }

        private void OnEnable()
        {
            SelectAction.action.performed += SelectPressed;
        }

        private void OnDisable()
        {
            SelectAction.action.performed -= SelectPressed;
        }

        // Set the hover state of the gaze pointer.
        private void SetHoverState(bool isHovering)
        {
            if (isHovering == IsHovering)
            {
                return;
            }

            if (!isHovering)
            {
                IsHovering = isHovering;
                ResetReticle();
            }
        }

        private void UpdateGazeCounter()
        {
            if (_xrGazeInteractor.TryGetCurrentUIRaycastResult(out RaycastResult RaycastResult, out int raycastEndpointIndex))
            {
                // Set the pointer position to the hit result position.
                SetPointerPosition(RaycastResult.worldPosition, RaycastResult.worldNormal);
                // Check if there is a Selectable component in the Game Object.
                var selectable = RaycastResult.gameObject.GetComponent<Selectable>();
                if (selectable != null)
                {
                    if (!selectable.IsInteractable())
                    {
                        SetHoverState(false);
                        return;
                    }

                    IsHovering = true;
                }

                // Check if there is a Selectable component in the parent of the Game ObjectCheck (specific for toggles).
                var selectableParent = RaycastResult.gameObject.GetComponentInParent<Selectable>();
                if (selectableParent != null)
                {
                    if (!selectableParent.IsInteractable())
                    {
                        SetHoverState(false);
                        return;
                    }

                    IsHovering = true;
                }

                // Reset the hover state if there are no selectable components in the Raycast Result.
                if (selectableParent == null && selectable == null)
                {
                    SetHoverState(false);
                }

                if (IsHovering)
                {
                    // Check for a click handler in the Raycast result
                    IPointerClickHandler clickHandler = RaycastResult.gameObject.GetComponentInParent<IPointerClickHandler>();
                    GetPointerEventData(RaycastResult.worldPosition, out PointerEventData pointerEventData);

                    // Reset and returns if the active click handler is null.
                    if (_activeClickHandler == null || _activeClickHandler != clickHandler)
                    {
                        _isSelectPressed = false;
                        _activeClickHandler = clickHandler;
                        ResetReticle();
                        return;
                    }

                    // Set the timer duration equal to the time of hover time to select in the XR Gaze Interator.
                    float gazeTimerDuration = _xrGazeInteractor.hoverTimeToSelect;
                    var continuousClick = false;

                    // Check if there is a GazeHoverOverride.
                    var gazeHoverOverride = RaycastResult.gameObject.GetComponentInParent<GazeHoverOverride>();
                    if (gazeHoverOverride != null)
                    {
                        gazeTimerDuration = gazeHoverOverride.GazeTimerDuration;
                        continuousClick = gazeHoverOverride.ContinuousClick;
                    }
                    // Add a delay before the time to select starts.
                    if (_safeTimerCurrent <= DelayGazeLoading)
                    {
                        _safeTimerCurrent += Time.deltaTime;
                        return;
                    }

                    // Fill the reticle outer ring as long as is set by the time to select in the XR Gaze Pointer.
                    if (_gazeTimerCurrent < gazeTimerDuration)
                    {
                        _gazeTimerCurrent += Time.deltaTime;
                        // Increase the fill amount by the normalized value (0.0 to 1.0).
                        ReticleOuterRing.fillAmount = _gazeTimerCurrent / gazeTimerDuration;
                    }

                    // Override click if there's a continuos click configured through the GazeHoverOverride.
                    if (continuousClick && ReticleOuterRing.fillAmount >= 1f)
                    {
                        _activeClickHandler.OnPointerClick(pointerEventData);
                        ReticleOuterRing.fillAmount = 1f;
                        return;
                    }

                    // Reset the reticle and executes the Click event on the active click handler.
                    if (ReticleOuterRing.fillAmount >= 1f || _isSelectPressed)
                    {
                        _activeClickHandler.OnPointerClick(pointerEventData);
                        ReticleOuterRing.fillAmount = 0f;
                        _gazeTimerCurrent = gazeTimerDuration;
                        _isSelectPressed = false;
                    }
                }
            }

            // Reset the pointer position to the default position when there is no hit.
            else
            {
                SetHoverState(false);
                var rayOriginTransform = _xrGazeInteractor.rayOriginTransform;
                SetPointerPosition(rayOriginTransform.position + (rayOriginTransform.forward * DefaultDistance), -rayOriginTransform.forward);
                _isSelectPressed = false;
            }
        }

        private void GetPointerEventData(in Vector2 RaycastPosition, out PointerEventData pointerEventData)
        {
            // Get the Pointer Event Data from the Gaze Interactor position to test against UI.
            pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = RaycastPosition;
        }

        private void SetPointerPosition(Vector3 position, Vector3 normal)
        {
            // Offset the position on Z to avoid z-fighting/clipping.
            ReticleGameObject.position = position + (normal * 0.1f);
            ReticleGameObject.rotation = Quaternion.LookRotation(-normal);
        }

        // Allow selecting with the controller.
        private void SelectPressed(InputAction.CallbackContext ctx)
        {
            _isSelectPressed = true;
        }

        // Reset the reticle UI.
        private void ResetReticle()
        {
            _gazeTimerCurrent = 0f;
            _safeTimerCurrent = 0f;
            ReticleOuterRing.fillAmount = 0f;
        }
    }
}
