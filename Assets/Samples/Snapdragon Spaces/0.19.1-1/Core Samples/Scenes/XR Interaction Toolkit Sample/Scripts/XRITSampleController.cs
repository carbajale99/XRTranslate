/******************************************************************************
 * File: XRITSampleController.cs
 * Copyright (c) 2021-2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class XRITSampleController : SampleController
    {
        public Text ScrollbarText;
        public Text TouchpadXText;
        public Text TouchpadYText;
        public InputActionReference TouchpadInputAction;
        public RectTransform TouchpadPositionIndicator;
        private readonly float _touchpadPositionIndicatorScaleFactor = 100f;

        public void OnScrollValueChanged(float value)
        {
            SendHapticImpulse(duration: 0.1f);
            ScrollbarText.text = value.ToString("#0.00");
        }

        private void Update()
        {
            var touchpadValue = TouchpadInputAction.action.ReadValue<Vector2>();
            TouchpadXText.text = touchpadValue.x.ToString("#0.00");
            TouchpadYText.text = touchpadValue.y.ToString("#0.00");
            TouchpadPositionIndicator.anchoredPosition = touchpadValue * _touchpadPositionIndicatorScaleFactor;
        }
    }
}
