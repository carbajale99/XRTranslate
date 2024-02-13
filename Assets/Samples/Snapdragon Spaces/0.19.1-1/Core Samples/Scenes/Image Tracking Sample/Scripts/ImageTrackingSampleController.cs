/******************************************************************************
 * File: ImageTrackingSampleController.cs
 * Copyright (c) 2022-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class ImageTrackingSampleController : SampleController
    {
        [Serializable]
        public struct TrackableInfo
        {
            public Text TrackingStatusText;
            public Text[] PositionTexts;
        }

        public ARTrackedImageManager arImageManager;
        public SpacesReferenceImageConfigurator referenceImageConfigurator;
        public Toggle dynamicModeToggle;
        public Toggle staticModeToggle;
        public Toggle adaptiveModeToggle;
        public TrackableInfo[] trackableInfos;
        private readonly string _referenceImageName = "Spaces Town";
        private readonly Dictionary<TrackableId, TrackableInfo> _trackedImages = new Dictionary<TrackableId, TrackableInfo>();

        public override void OnEnable()
        {
            base.OnEnable();
            arImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            if (referenceImageConfigurator.HasReferenceImageTrackingMode(_referenceImageName))
            {
                switch (referenceImageConfigurator.GetTrackingModeForReferenceImage(_referenceImageName))
                {
                    case SpacesImageTrackingMode.STATIC:
                        staticModeToggle.isOn = true;
                        break;
                    case SpacesImageTrackingMode.DYNAMIC:
                        dynamicModeToggle.isOn = true;
                        break;
                    case SpacesImageTrackingMode.ADAPTIVE:
                        adaptiveModeToggle.isOn = true;
                        break;
                    case SpacesImageTrackingMode.INVALID:
                        Debug.LogWarning($"Invalid tracking mode for reference image: {_referenceImageName}");
                        break;
                }
            }
            else
            {
                Debug.LogWarning($"Could not find reference image: {_referenceImageName} ");
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            arImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            foreach (var trackedImage in _trackedImages)
            {
                referenceImageConfigurator.StopTrackingImageInstance(_referenceImageName, trackedImage.Key);
            }
        }

        public void OnToggleDynamicTrackingMode(bool enabled)
        {
            if (enabled)
            {
                foreach (var trackedImage in _trackedImages)
                {
                    referenceImageConfigurator.StopTrackingImageInstance(_referenceImageName, trackedImage.Key);
                }

                referenceImageConfigurator.SetTrackingModeForReferenceImage(_referenceImageName, SpacesImageTrackingMode.DYNAMIC);
            }
        }

        public void OnToggleStaticTrackingMode(bool enabled)
        {
            if (enabled)
            {
                foreach (var trackedImage in _trackedImages)
                {
                    referenceImageConfigurator.StopTrackingImageInstance(_referenceImageName, trackedImage.Key);
                }

                referenceImageConfigurator.SetTrackingModeForReferenceImage(_referenceImageName, SpacesImageTrackingMode.STATIC);
            }
        }

        public void OnToggleAdaptiveTrackingMode(bool enabled)
        {
            if (enabled)
            {
                foreach (var trackedImage in _trackedImages)
                {
                    referenceImageConfigurator.StopTrackingImageInstance(_referenceImageName, trackedImage.Key);
                }

                referenceImageConfigurator.SetTrackingModeForReferenceImage(_referenceImageName, SpacesImageTrackingMode.ADAPTIVE);
            }
        }

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
        {
            foreach (var trackedImage in args.added)
            {
                if (trackedImage.referenceImage.name == _referenceImageName)
                {
                    _trackedImages.Add(trackedImage.trackableId, trackableInfos[0]);
                    UpdateTrackedText(trackedImage, trackableInfos[0]);
                }
            }

            foreach (var trackedImage in args.updated)
            {
                if (_trackedImages.TryGetValue(trackedImage.trackableId, out TrackableInfo info))
                {
                    UpdateTrackedText(trackedImage, info);
                }
            }

            foreach (var trackedImage in args.removed)
            {
                if (_trackedImages.TryGetValue(trackedImage.trackableId, out TrackableInfo info))
                {
                    info.TrackingStatusText.text = "None";
                    info.PositionTexts[0].text = "0.00";
                    info.PositionTexts[1].text = "0.00";
                    info.PositionTexts[2].text = "0.00";
                    _trackedImages.Remove(trackedImage.trackableId);
                }
            }
        }

        // Updates Tracked Image UI texts.
        private void UpdateTrackedText(ARTrackedImage trackedImage, TrackableInfo info)
        {
            Vector3 position = trackedImage.transform.position;
            info.TrackingStatusText.text = trackedImage.trackingState.ToString();
            info.PositionTexts[0].text = position.x.ToString("#0.00");
            info.PositionTexts[1].text = position.y.ToString("#0.00");
            info.PositionTexts[2].text = position.z.ToString("#0.00");
        }

        protected override bool CheckSubsystem()
        {
            return arImageManager.subsystem?.running ?? false;
        }
    }
}
