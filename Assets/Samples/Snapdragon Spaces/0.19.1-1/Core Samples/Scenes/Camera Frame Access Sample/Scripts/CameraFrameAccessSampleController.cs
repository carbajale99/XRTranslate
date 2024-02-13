/******************************************************************************
 * File: CameraFrameAccessSampleController.cs
 * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class CameraFrameAccessSampleController : SampleController
    {
        [Header("Camera Feed")] public RawImage CameraRawImage;

        public bool RenderUsingYUVPlanes;

        [Header("GUI Bindings")] public Text[] ResolutionTexts;

        public Text[] FocalLengthTexts;
        public Text[] PrincipalPointTexts;
        public Text ConfigNotFoundText;

        private NativeArray<XRCameraConfiguration> _cameraConfigs;
        private ARCameraManager _cameraManager;
        private SpacesARCameraManagerConfig _cameraManagerConfig;
        private Texture2D _cameraTexture;
        private float _defaultAspectRatio;
        private bool _configurationFound;
        private bool _feedPaused;
        private XRCameraIntrinsics _intrinsics;
        private XRCpuImage _lastCpuImage;
        private Vector2 _maxTextureSize;

        public void Awake()
        {
            _cameraManager = FindObjectOfType<ARCameraManager>();
            _cameraManagerConfig = FindObjectOfType<SpacesARCameraManagerConfig>();
        }

        public override void Start()
        {
            base.Start();

            if (!SubsystemChecksPassed)
            {
                return;
            }

            _configurationFound = FindSupportedConfiguration();
            if (!_configurationFound)
            {
                OnConfigurationNotFound();
                return;
            }

            _cameraManager.frameReceived += OnFrameReceived;

            _maxTextureSize = CameraRawImage.rectTransform.sizeDelta;
            _defaultAspectRatio = _maxTextureSize.x / _maxTextureSize.y;
        }

        private void OnFrameReceived(ARCameraFrameEventArgs args)
        {
            if (_feedPaused)
            {
                return;
            }

            if (!_cameraManager.TryAcquireLatestCpuImage(out _lastCpuImage))
            {
                Debug.Log("Failed to acquire latest cpu image.");
                return;
            }

            UpdateCameraTexture(_lastCpuImage, RenderUsingYUVPlanes);
            // Update intrinsics on every frame, as intrinsics can change over time
            UpdateCameraIntrinsics();
        }

        private unsafe void UpdateCameraTexture(XRCpuImage image, bool convertYuvManually)
        {
            var format = TextureFormat.RGBA32;

            if (_cameraTexture == null || _cameraTexture.width != image.width || _cameraTexture.height != image.height)
            {
                _cameraTexture = new Texture2D(image.width, image.height, format, false);
                ResizeCameraFeed(image.dimensions);
            }

            var conversionParams = new XRCpuImage.ConversionParams(image, format);
            var rawTextureData = _cameraTexture.GetRawTextureData<byte>();
            var rawTexturePtr = new IntPtr(rawTextureData.GetUnsafePtr());

            if (convertYuvManually)
            {
                ConvertYuvImageIntoBuffer(image, rawTexturePtr, format);
            }
            else
            {
                try
                {
                    image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
                }
                finally
                {
                    image.Dispose();
                }
            }

            _cameraTexture.Apply();
            CameraRawImage.texture = _cameraTexture;
        }

        private void ConvertYuvImageIntoBuffer(XRCpuImage image, IntPtr targetBuffer, TextureFormat format)
        {
            var bufferSize = image.height * image.width * 4;
            var rgbBuffer = new byte[bufferSize];

            // Populate buffer with YUV => RGB conversion

            var yPlane = image.GetPlane(0);
            var uvPlane = image.GetPlane(1);

            for (int row = 0; row < image.height; row++)
            {
                for (int col = 0; col < image.width; col++)
                {
                    var y = yPlane.data[row * image.width + col];

                    var offset = (row / 2) * image.width + (col / 2) * 2;
                    sbyte u = (sbyte) (uvPlane.data[offset] - 128);
                    sbyte v = (sbyte) (uvPlane.data[offset + 1] - 128);

                    // YUV NV12 to RGB conversion (A3 wrongly reports NV21, no need to swap U & V)
                    // https://en.wikipedia.org/wiki/YUV#Y%E2%80%B2UV420sp_(NV21)_to_RGB_conversion_(Android)

                    var r = y + (1.370705f * v);
                    var g = y - (0.698001f * v) - (0.337633f * u);
                    var b = y + (1.732446f * u);

                    r = r > 255 ? 255 : r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b < 0 ? 0 : b;

                    int pixelIndex = ((image.height - row - 1) * image.width) + col;

                    switch (format)
                    {
                        case TextureFormat.RGB24:
                            rgbBuffer[4 * pixelIndex] = (byte)r;
                            rgbBuffer[(4 * pixelIndex) + 1] = (byte)g;
                            rgbBuffer[(4 * pixelIndex) + 2] = (byte)b;
                            break;
                        case TextureFormat.RGBA32:
                            rgbBuffer[4 * pixelIndex] = (byte)r;
                            rgbBuffer[(4 * pixelIndex) + 1] = (byte)g;
                            rgbBuffer[(4 * pixelIndex) + 2] = (byte)b;
                            rgbBuffer[(4 * pixelIndex) + 3] = 255;
                            break;
                        case TextureFormat.BGRA32:
                            rgbBuffer[4 * pixelIndex] = (byte)b;
                            rgbBuffer[(4 * pixelIndex) + 1] = (byte)g;
                            rgbBuffer[(4 * pixelIndex) + 2] = (byte)r;
                            rgbBuffer[(4 * pixelIndex) + 3] = 255;
                            break;
                    }
                }
            }

            Marshal.Copy(rgbBuffer, 0, targetBuffer, bufferSize);
        }

        private void UpdateCameraIntrinsics()
        {
            if (!_cameraManager.TryGetIntrinsics(out _intrinsics))
            {
                Debug.Log("Failed to acquire camera intrinsics.");
                return;
            }

            ResolutionTexts[0].text = _intrinsics.resolution.x.ToString();
            ResolutionTexts[1].text = _intrinsics.resolution.y.ToString();
            FocalLengthTexts[0].text = _intrinsics.focalLength.x.ToString("#0.00");
            FocalLengthTexts[1].text = _intrinsics.focalLength.y.ToString("#0.00");
            PrincipalPointTexts[0].text = _intrinsics.principalPoint.x.ToString("#0.00");
            PrincipalPointTexts[1].text = _intrinsics.principalPoint.y.ToString("#0.00");
        }

        private void ResizeCameraFeed(Vector2Int outputDimensions)
        {
            var outputAspectRatio = outputDimensions.x / (float)outputDimensions.y;
            if (outputAspectRatio > _defaultAspectRatio)
            {
                CameraRawImage.rectTransform.sizeDelta = new Vector2(_maxTextureSize.x, _maxTextureSize.x / outputAspectRatio);
            }
            else
            {
                CameraRawImage.rectTransform.sizeDelta = new Vector2(_maxTextureSize.y * outputAspectRatio, _maxTextureSize.y);
            }
        }

        private bool FindSupportedConfiguration()
        {
            _cameraConfigs = _cameraManager.GetConfigurations(Allocator.Persistent);
            return _cameraConfigs.Length > 0;
        }

        private void OnConfigurationNotFound()
        {
            foreach (var content in ContentOnPassed)
            {
                content.SetActive(false);
            }

            foreach (var content in ContentOnFailed)
            {
                content.SetActive(true);
            }

            ConfigNotFoundText.text = "Could not find supported frame configuration.";
        }

        public void OnPausePress()
        {
            _feedPaused = true;
        }

        public void OnResumePress()
        {
            _feedPaused = false;
        }

        protected override bool CheckSubsystem()
        {
            return _cameraManager.subsystem?.running ?? false;
        }
    }
}
