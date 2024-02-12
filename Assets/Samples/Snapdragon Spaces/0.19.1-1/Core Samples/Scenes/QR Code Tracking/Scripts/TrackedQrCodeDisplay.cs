/******************************************************************************
 * File: TrackedQrCodeDisplay.cs
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 * Confidential and Proprietary - Qualcomm Technologies, Inc.
 *
 ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using Qualcomm.Snapdragon.Spaces;
using UnityEngine;
using UnityEngine.UI;

public class TrackedQrCodeDisplay : MonoBehaviour
{
    public SpacesARMarker arMarker;
    public Text trackableIdText;
    public Text dataStringText;

    void Update()
    {
        trackableIdText.text = arMarker.trackableId.ToString();
        dataStringText.text = arMarker.Data;
    }
}
