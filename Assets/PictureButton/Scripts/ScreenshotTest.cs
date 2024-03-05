/*using Qualcomm.Snapdragon.Spaces;
using Qualcomm.Snapdragon.Spaces.Samples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;


namespace Qualcomm.Snapdragon.Spaces.Samples
{

    public class ScreenshotTest : MonoBehaviour
    {

        [Tooltip("Whether the Game Object should follow or not the camera movement.")]
        public bool FollowGaze = false;
        [Tooltip("Target distance between the camera and the Game Object.")]
        public float TargetDistance = 10.0f;
        [Tooltip("Smoothness on the movement following the Game Object.")]
        public float MovementSmoothness = 0.2f;
        [Tooltip("Vertical follow threshold.")]
        public float VerticalBias = 0.8f;
        [Tooltip("Horizontal follow threshold.")]
        public float HorizontalBias = 0.5f;
        private Transform _arCameraTransform;
        private Camera _arCamera;

        private ARCameraManager arCameraManager;
        public GameObject testImage;
        public GameObject textMeshPro;


        private Texture2D camTexture;
        private XRCpuImage cpuImage;

        private WebAPI webAPI;



        // Start is called before the first frame update
        void Start()
        {


            _arCamera = OriginLocationUtility.GetOriginCamera();
            _arCameraTransform = _arCamera.transform;

            arCameraManager = _arCamera.GetComponent<ARCameraManager>();

            webAPI = new WebAPI();


        }

        private void Update()
        {
            if (FollowGaze)
            {
                AdjustPanelPosition();
            }

        }

        public void buttonclick()
        {

            StartCoroutine(CaptureImage());

        }

        private IEnumerator CaptureImage()
        {
            yield return new WaitForEndOfFrame();

            cpuImage = new XRCpuImage();
            if (!arCameraManager.TryAcquireLatestCpuImage(out cpuImage))
            {
                textMeshPro.GetComponent<TextMeshProUGUI>().text = "Failed to acquire latest cpu image.";
                yield return new WaitForEndOfFrame();
            }

            UpdateCameraTexture(cpuImage);


        }


        private unsafe void UpdateCameraTexture(XRCpuImage image)
        {
            var format = TextureFormat.RGBA32;

            if (camTexture == null || camTexture.width != image.width || camTexture.height != image.height)
            {
                camTexture = new Texture2D(image.width, image.height, format, false);
            }

            var conversionParams = new XRCpuImage.ConversionParams(image, format);
            var rawTextureData = camTexture.GetRawTextureData<byte>();

            try
            {
                image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
            }
            finally
            {

                image.Dispose();
            }

            camTexture.Apply();


            string fileName = "translate.png";

            string path = Path.Combine(Application.persistentDataPath, fileName);

            byte[] bytes = camTexture.EncodeToPNG();

            File.WriteAllBytes(path, bytes);

            string imageToText = webAPI.imageToText(path);

            byte[] pngImageByteArray = null;

            pngImageByteArray = File.ReadAllBytes(path);


            textMeshPro.GetComponent<TextMeshProUGUI>().text = imageToText;

            Texture2D tempTexture = new Texture2D(image.width, image.height, format, false);
            tempTexture.LoadImage(pngImageByteArray);
            testImage.GetComponent<RawImage>().texture = tempTexture;


        }



        private void AdjustPanelPosition()
        {
            var headPosition = _arCameraTransform.position;
            var gazeDirection = _arCameraTransform.forward;
            var direction = (transform.position - headPosition).normalized;
            var targetPosition = headPosition + (gazeDirection * TargetDistance);
            var targetDirection = (targetPosition - headPosition).normalized;
            var eulerAngles = Quaternion.LookRotation(direction).eulerAngles;
            var targetEulerAngles = Quaternion.LookRotation(targetDirection).eulerAngles;
            var verticalHalfAngle = _arCamera.fieldOfView * VerticalBias;
            eulerAngles.x += GetAdjustedDelta(targetEulerAngles.x - eulerAngles.x, verticalHalfAngle);
            var horizontalHalfAngle = _arCamera.fieldOfView * HorizontalBias * _arCamera.aspect;
            eulerAngles.y += GetAdjustedDelta(targetEulerAngles.y - eulerAngles.y, horizontalHalfAngle);
            targetPosition = headPosition + (Quaternion.Euler(eulerAngles) * Vector3.forward * TargetDistance);
            //transform.position = Vector3.Lerp(transform.position, targetPosition, MovementSmoothness);
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - headPosition), MovementSmoothness);
        }

        // Returns the normalized delta to a certain threshold, if it exceeds that threshold. Otherwise return 0.
        private float GetAdjustedDelta(float angle, float threshold)
        {
            // Normalize angle to be between 0 and 360.
            angle = ((540f + angle) % 360f) - 180f;
            if (Mathf.Abs(angle) > threshold)
            {
                return -angle / Mathf.Abs(angle) * (threshold - Mathf.Abs(angle));
            }

            return 0f;
        }

    }
}
*/