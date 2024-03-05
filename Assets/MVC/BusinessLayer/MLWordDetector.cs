using System.Collections;
using System;
using System.IO;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Drawing; // Remember to add a reference to System.Drawing.dll
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections.LowLevel.Unsafe;
using Qualcomm.Snapdragon.Spaces;
using UnityEngine.UI;
using TMPro;


namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class BoundingBox
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float CenterX { get; set; }
        public float CenterY { get; set; }
    }

    public class MLWordDetector : MonoBehaviour
    {
        private Camera _arCamera;

        private ARCameraManager arCameraManager;

        private Transform _arCameraTransform;

        private AspectMode aspectMode = AspectMode.Fill;

        public GameObject testText;

        public GameObject testTexture;

        private WebAPI webAPI = new WebAPI();


        void Start()
        {
            _arCamera = OriginLocationUtility.GetOriginCamera();
            _arCameraTransform = _arCamera.transform;

            arCameraManager = _arCamera.GetComponent<ARCameraManager>();

        }

        private int frameSkipCount = 0;
        private const int skipFrames = 10; // Adjust based on performance

        unsafe void Update()
        {

            if (frameSkipCount < skipFrames)
            {
                frameSkipCount++;
                return;
            }
            frameSkipCount = 0;

            // Example: Get camera image from ARCameraManager
            XRCpuImage image;
            if (arCameraManager.TryAcquireLatestCpuImage(out image)) //if camera avail
            {


            }


        }
    }
}