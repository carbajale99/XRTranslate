/******************************************************************************
  * File: AnchorSampleController.cs
  * Copyright (c)2022-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
  *
  ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class AnchorSampleController : SampleController
    {
        public ARAnchorManager AnchorManager;
        public GameObject GizmoTransparent;
        public GameObject GizmoSurface;
        public GameObject GizmoTrackedAnchor;
        public GameObject GizmoUntrackedAnchor;
        public GameObject GizmoSavedAddition;
        public GameObject GizmoNotSavedAddition;
        public GameObject CreateButtonUI;
        public InputActionReference TriggerAction;
        public Toggle SaveNewAnchorsToggle;
        public Toggle UseSurfacePlacementToggle;
        public Text NumberOfAnchorsStoredText;
        public float PlacementDistance = 1f;
        public bool RestrictRaycastDistance;
        private readonly List<GameObject> _anchorGizmos = new List<GameObject>();
        private readonly List<GameObject> _sessionGizmos = new List<GameObject>();
        private SpacesAnchorStore _anchorStore;
        private bool _placeAnchorAtRaycastHit;
        private bool _canPlaceAnchorGizmos = true;
        private GameObject _indicatorGizmo;
        private GameObject _transparentGizmo;
        private GameObject _surfaceGizmo;
        private ARRaycastManager _raycastManager;
        private bool _saveAnchorsToStore => SaveNewAnchorsToggle.isOn;
        private bool _useSurfacePlacement => UseSurfacePlacementToggle.isOn;
        private UnityAction<bool> _onToggleChangedAction => _ => SendHapticImpulse();
        private readonly string _anchorGizmoName = "AnchorGizmo";
        private readonly string _additionName = "Addition";

        public override void Start()
        {
            base.Start();
            if (!SubsystemChecksPassed)
            {
                return;
            }

            _indicatorGizmo = new GameObject("IndicatorGizmo");
            _transparentGizmo = Instantiate(GizmoTransparent, _indicatorGizmo.transform.position, Quaternion.identity, _indicatorGizmo.transform);
            _surfaceGizmo = Instantiate(GizmoSurface, _indicatorGizmo.transform.position, Quaternion.identity, _indicatorGizmo.transform);
            _surfaceGizmo.SetActive(false);
            CreateButtonUI.SetActive(InteractionManager.InputType == InputType.GazePointer);
            NumberOfAnchorsStoredText.text = _anchorStore.GetSavedAnchorNames().Length.ToString();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (!SubsystemChecksPassed)
            {
                return;
            }

            SaveNewAnchorsToggle.onValueChanged.AddListener(_onToggleChangedAction);
            UseSurfacePlacementToggle.onValueChanged.AddListener(_onToggleChangedAction);
            AnchorManager.anchorsChanged += OnAnchorsChanged;
            InteractionManager.onInputTypeSwitch += UpdateCreateButtonUI;
            TriggerAction.action.performed += OnTriggerAction;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (!SubsystemChecksPassed)
            {
                return;
            }

            SaveNewAnchorsToggle.onValueChanged.RemoveListener(_onToggleChangedAction);
            UseSurfacePlacementToggle.onValueChanged.RemoveListener(_onToggleChangedAction);
            AnchorManager.anchorsChanged -= OnAnchorsChanged;
            InteractionManager.onInputTypeSwitch -= UpdateCreateButtonUI;
            TriggerAction.action.performed -= OnTriggerAction;
        }

        public void OnCreateButtonClicked()
        {
            SendHapticImpulse();
            InstantiateGizmos();
        }

        public void InstantiateGizmos()
        {
            var targetPosition = _indicatorGizmo.transform.position;
            var sessionGizmo = _placeAnchorAtRaycastHit ? Instantiate(GizmoSurface, targetPosition, Quaternion.identity) : Instantiate(GizmoTransparent, targetPosition, Quaternion.identity);
            _sessionGizmos.Add(sessionGizmo);
            var anchorGizmo = new GameObject
            {
                transform =
                {
                    position = targetPosition,
                    rotation = Quaternion.identity
                }
            };
            var anchor = anchorGizmo.AddComponent<ARAnchor>();
            var gizmo = Instantiate(GizmoUntrackedAnchor, anchor.transform);
            gizmo.name = _anchorGizmoName;
            if (_saveAnchorsToStore)
            {
                var addition = Instantiate(GizmoNotSavedAddition, anchor.transform);
                addition.name = _additionName;
                _anchorStore.SaveAnchorWithResult(anchor, result =>
                    {
                        Debug.Log("Save Anchor result: " + result);
                        NumberOfAnchorsStoredText.text = _anchorStore.GetSavedAnchorNames().Length.ToString();
                        UpdateGizmoSavedAddition(anchor);
                    });
            }
        }

        public void LoadAllSavedAnchors()
        {
            SendHapticImpulse();
            _anchorStore.LoadAllSavedAnchors(success =>
            {
                Debug.Log("Load Anchor Success: " + success);
            });
        }

        public void ClearAnchorStore()
        {
            SendHapticImpulse();
            _anchorStore.ClearStore();
            NumberOfAnchorsStoredText.text = _anchorStore.GetSavedAnchorNames().Length.ToString();
        }

        public void DestroyGizmos()
        {
            SendHapticImpulse();
            foreach (var anchorGizmo in _anchorGizmos.ToList())
            {
                Destroy(anchorGizmo);
            }

            foreach (var gizmo in _sessionGizmos.ToList())
            {
                Destroy(gizmo);
            }

            _sessionGizmos.Clear();
        }

        public void OnPointerEnterEvent()
        {
            _canPlaceAnchorGizmos = false;
        }

        public void OnPointerExitEvent()
        {
            _canPlaceAnchorGizmos = true;
        }

        private void Awake()
        {
            _anchorStore = FindObjectOfType<SpacesAnchorStore>();
            _raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        private void Update()
        {
            if (!SubsystemChecksPassed)
            {
                return;
            }

            if (_useSurfacePlacement)
            {
                _placeAnchorAtRaycastHit = false;
                Ray ray = new Ray(InteractionManager.ArCameraTransform.position, InteractionManager.ArCameraTransform.forward);
                List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
                if (_raycastManager.Raycast(ray, hitResults))
                {
                    _placeAnchorAtRaycastHit = !RestrictRaycastDistance || (hitResults[0].pose.position - InteractionManager.ArCameraTransform.position).magnitude < PlacementDistance;
                }

                if (_placeAnchorAtRaycastHit)
                {
                    if (!_surfaceGizmo.activeSelf)
                    {
                        _surfaceGizmo.SetActive(true);
                        _transparentGizmo.SetActive(false);
                    }

                    _indicatorGizmo.transform.position = hitResults[0].pose.position;
                    return;
                }
            }

            if (_surfaceGizmo.activeSelf)
            {
                _surfaceGizmo.SetActive(false);
                _transparentGizmo.SetActive(true);
            }

            _indicatorGizmo.transform.position = InteractionManager.ArCameraTransform.position + (InteractionManager.ArCameraTransform.forward * PlacementDistance);
        }

        private void OnTriggerAction(InputAction.CallbackContext context)
        {
            if (!_canPlaceAnchorGizmos)
            {
                return;
            }

            SendHapticImpulse();
            InstantiateGizmos();
        }

        private void OnAnchorsChanged(ARAnchorsChangedEventArgs args)
        {
            foreach (var anchor in args.added)
            {
                _anchorGizmos.Add(anchor.gameObject);
            }

            foreach (var anchor in args.updated)
            {
                // Remove old anchor gizmos
                // NOTE(SS): Sometimes Destroy takes some time, therefore there may be more than 1 child gizmo, so we need to remove all children that are gizmos
                foreach (Transform child in anchor.transform)
                {
                    var go = child.gameObject;
                    if (go.name == _anchorGizmoName)
                    {
                        go.SetActive(false); // Destroy takes some time, so turn it off to not see it
                        Destroy(go);
                    }
                }

                var newGizmo = Instantiate(anchor.trackingState == TrackingState.None ? GizmoUntrackedAnchor : GizmoTrackedAnchor, anchor.transform);
                newGizmo.name = _anchorGizmoName;

                UpdateGizmoSavedAddition(anchor);
            }

            foreach (var anchor in args.removed)
            {
                _anchorGizmos.Remove(anchor.gameObject);
            }
        }

        private void UpdateGizmoSavedAddition(ARAnchor anchor)
        {
            if (_anchorStore.GetSavedAnchorNameFromARAnchor(anchor) != string.Empty)
            {
                // Remove old cube additions
                // NOTE(SS): Same reasoning as above, there may be multiple cube children
                foreach (Transform child in anchor.transform)
                {
                    var go = child.gameObject;
                    if (go.name == _additionName)
                    {
                        go.SetActive(false);
                        Destroy(go);
                    }
                }

                var newAddition = Instantiate(GizmoSavedAddition, anchor.transform);
                newAddition.name = _additionName;
            }
        }

        private IEnumerator DestroyGizmosCoroutine()
        {
            yield return new WaitForEndOfFrame();
            foreach (var anchorGizmo in _anchorGizmos.ToList())
            {
                Destroy(anchorGizmo);
            }

            foreach (var gizmo in _sessionGizmos.ToList())
            {
                Destroy(gizmo);
            }

            _sessionGizmos.Clear();
        }

        private void UpdateCreateButtonUI(InputType inputType)
        {
            CreateButtonUI.SetActive(inputType == InputType.GazePointer);
        }

        protected override bool CheckSubsystem()
        {
            if (_baseRuntimeFeature.CheckServicesCameraPermissions())
            {
                return (AnchorManager.subsystem?.running ?? false) &&
                    (_raycastManager.subsystem?.running ?? false);
            }

            Debug.LogWarning("Snapdragon Spaces Services has no camera permissions! Spatial Anchors and Hit Testing features disabled.");
            return false;
        }
    }
}
