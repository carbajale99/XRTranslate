/******************************************************************************
 * File: HitTestingSampleController.cs
 * Copyright (c)2021-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class HitTestingSampleController : SampleController
    {
        public GameObject HitIndicator;
        public GameObject NoHitIndicator;
        private ARRaycastManager _raycastManager;
        private GameObject _activeIndicator;
        private bool _isHit;
        private Vector3 _desiredPosition;
        private Quaternion _desiredRotation;

        public void Awake()
        {
            _raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        public override void Start()
        {
            base.Start();
            if (!SubsystemChecksPassed)
            {
                return;
            }

            _activeIndicator = NoHitIndicator;
            _activeIndicator.SetActive(true);
        }

        public void CastRay()
        {
            Ray ray = new Ray(InteractionManager.ArCameraTransform.position, InteractionManager.ArCameraTransform.forward);
            List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
            if (_raycastManager.Raycast(ray, hitResults))
            {
                _desiredPosition = hitResults[0].pose.position;
                _desiredRotation = hitResults[0].pose.rotation;
                if (!_isHit)
                {
                    _activeIndicator.SetActive(false);
                    _activeIndicator = HitIndicator;
                    _activeIndicator.SetActive(true);
                    _isHit = true;
                }
            }
            else
            {
                _desiredPosition = InteractionManager.ArCameraTransform.position + InteractionManager.ArCameraTransform.forward;
                _desiredRotation = Quaternion.identity;
                if (_isHit)
                {
                    _activeIndicator.SetActive(false);
                    _activeIndicator = NoHitIndicator;
                    _activeIndicator.SetActive(true);
                    _isHit = false;
                }
            }
        }

        private void Update()
        {
            if (!SubsystemChecksPassed)
            {
                return;
            }

            CastRay();
            _activeIndicator.transform.position = _desiredPosition;
            _activeIndicator.transform.rotation = _desiredRotation;
        }

        protected override bool CheckSubsystem()
        {
            return _raycastManager.subsystem?.running ?? false;
        }
    }
}
