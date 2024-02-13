/******************************************************************************
 * File: PinIconSwitcher.cs
 * Copyright (c) 2022-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;

public class PinIconSwitcher : MonoBehaviour
{
    [Tooltip("Pin icon reference.")]
    public GameObject PinIcon;
    [Tooltip("Unpin icon reference.")]
    public GameObject UnpinIcon;

    public void PinUI(bool pin)
    {
        PinIcon.SetActive(pin);
        UnpinIcon.SetActive(!pin);
    }
}
