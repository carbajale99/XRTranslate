/******************************************************************************
 * File: GazeHoverOverride.cs
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class GazeHoverOverride : MonoBehaviour
    {
        public float GazeTimerDuration = 2f;
        public bool ContinuousClick = true;

        public void SetContinuousClick(bool isContinuousClickEnable)
        {
            ContinuousClick = isContinuousClickEnable;
        }
    }
}
