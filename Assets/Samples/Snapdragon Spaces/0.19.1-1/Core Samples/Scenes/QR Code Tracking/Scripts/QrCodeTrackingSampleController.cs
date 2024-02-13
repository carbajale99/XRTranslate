/******************************************************************************
 * File: QrCodeTrackingSampleController.cs
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
    public class QrCodeTrackingSampleController : SampleController
    {
        public SpacesQrCodeManager arQrCodeManager;
        public Text markerWidthText;
        public Text markerHeightText;

        public Text minQrCodeVersionText;
        public Text maxQrCodeVersionText;

        public override void OnEnable()
        {
            base.OnEnable();
            UpdateQrCodeManagerUI();
        }

        private void UpdateQrCodeManagerUI()
        {
            markerWidthText.text = arQrCodeManager.markerSize.x.ToString();
            markerHeightText.text = arQrCodeManager.markerSize.y.ToString();
            minQrCodeVersionText.text = arQrCodeManager.minQrVersion.ToString();
            maxQrCodeVersionText.text = arQrCodeManager.maxQrVersion.ToString();
        }
        protected override bool CheckSubsystem()
        {
            return arQrCodeManager.subsystem?.running ?? false;
        }
    }
}
