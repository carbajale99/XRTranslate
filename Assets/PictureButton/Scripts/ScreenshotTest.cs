using Qualcomm.Snapdragon.Spaces;
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


        public IEnumerator takeScreenshot()
        {

            yield return new WaitForEndOfFrame();


            int width = Screen.width;
            int height = Screen.height;
            RenderTexture rt = new RenderTexture(width, height, 24);

            _arCamera.targetTexture = rt;

            // The Render Texture in RenderTexture.active is the one
            // that will be read by ReadPixels.
            //var currentRT = RenderTexture.active;

            // Render the camera's view.

            // Make a new texture and read the active Render Texture into it.
            Texture2D image = new Texture2D(width, height);

            _arCamera.Render();
            RenderTexture.active = rt;

            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            image.Apply();

            _arCamera.targetTexture = null;

            RenderTexture.active = null;

            //Destroy(rt);

            // Replace the original active Render Texture.

            //RenderTexture.active = currentRT;

            byte[] bytes = image.EncodeToPNG();
            Debug.Log(Application.persistentDataPath);
            string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            string filePath = Path.Combine(Application.dataPath, fileName);
            File.WriteAllBytes(filePath, bytes);

            //Destroy(image);


            //Destroy(rt);
            //Destroy(image);



            //if (currentRT == null)
            //{
            //}
            //camRawImage.texture = image;
        }


        public void imageTaker()
        {
            // Acquire an XRCpuImage
            if (!arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
                return;

            // Set up our conversion params
            var conversionParams = new XRCpuImage.ConversionParams
            {
                // Convert the entire image
                inputRect = new RectInt(0, 0, image.width, image.height),

                // Output at full resolution
                outputDimensions = new Vector2Int(image.width, image.height),

                // Convert to RGBA format
                outputFormat = TextureFormat.RGBA32,

                // Flip across the vertical axis (mirror image)
                transformation = XRCpuImage.Transformation.MirrorY
            };

            // Create a Texture2D to store the converted image
            var texture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

            // Texture2D allows us write directly to the raw texture data as an optimization
            var rawTextureData = texture.GetRawTextureData<byte>();
            try
            {
                unsafe
                {
                    // Synchronously convert to the desired TextureFormat
                    image.Convert(
                        conversionParams,
                        new IntPtr(rawTextureData.GetUnsafePtr()),
                        rawTextureData.Length);
                }
            }
            finally
            {
                // Dispose the XRCpuImage after we're finished to prevent any memory leaks
                image.Dispose();
            }

            // Apply the converted pixel data to our texture
            texture.Apply();

            //camRawImage.texture = texture;

        }



        //public void takeScreenshot()
        //{

        //    bool captured = arCamera.GetComponent<ARCameraManager>().TryAcquireLatestCpuImage(out cpuImage);

        //    debug.GetComponent<TextMeshProUGUI>().text = captured.ToString();

        //    RawImage rawImage = image.GetComponent<RawImage>();

        //    // Get the texture associated with the UI.RawImage that we wish to display on screen.
        //    var texture = rawImage.texture as Texture2D;

        //    // If the texture hasn't yet been created, or if its dimensions have changed, (re)create the texture.
        //    // Note: Although texture dimensions do not normally change frame-to-frame, they can change in response to
        //    //    a change in the camera resolution (for camera images) or changes to the quality of the human depth
        //    //    and human stencil buffers.
        //    if (texture == null || texture.width != cpuImage.width || texture.height != cpuImage.height)
        //    {
        //        texture = new Texture2D(cpuImage.width, cpuImage.height, cpuImage.format.AsTextureFormat(), false);
        //        rawImage.texture = texture;
        //    }

        //    // For display, we need to mirror about the vertical access.
        //    var conversionParams = new XRCpuImage.ConversionParams(cpuImage, cpuImage.format.AsTextureFormat(), XRCpuImage.Transformation.MirrorY);

        //    //Debug.Log("Texture format: " + cpuImage.format.AsTextureFormat()); -> RFloat

        //    // Get the Texture2D's underlying pixel buffer.
        //    var rawTextureData = texture.GetRawTextureData<byte>();

        //    // Make sure the destination buffer is large enough to hold the converted data (they should be the same size)
        //    Debug.Assert(rawTextureData.Length == cpuImage.GetConvertedDataSize(conversionParams.outputDimensions, conversionParams.outputFormat),
        //        "The Texture2D is not the same size as the converted data.");

        //    // Perform the conversion.
        //    cpuImage.Convert(conversionParams, rawTextureData);

        //    // "Apply" the new pixel data to the Texture2D.
        //    texture.Apply();

        //    //image.GetComponent<RawImage>().texture = texture;

        //}

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
