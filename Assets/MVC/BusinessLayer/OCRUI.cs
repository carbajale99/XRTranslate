using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Qualcomm.Snapdragon.Spaces;
using System.IO;
using System;
using TMPro;
using UnityEngine.UI;
using Unity.Collections.LowLevel.Unsafe;


public class OCRUI : MonoBehaviour
{
    private WebAPI webAPI = new WebAPI();

    private ConvertedText convertedText = new ConvertedText();

    private XRCpuImage cpuImage;
    private Texture2D camTexture;

    private Transform _arCameraTransform;
    private Camera _arCamera;

    private ARCameraManager arCameraManager;

    private float conversionX = 3.9f;

    private float conversionY = 2.1f;

    public GameObject testText;

    public GameObject coverPrefab;
    public GameObject canvas;


    void Start()
    {
        _arCamera = OriginLocationUtility.GetOriginCamera();
        _arCameraTransform = _arCamera.transform;

        arCameraManager = _arCamera.GetComponent<ARCameraManager>();

        webAPI = new WebAPI();
    }

    public void ocrClick()
    {
        //var items = ocrConversion();
        //Debug.Log(items);

        StartCoroutine(takeImage());

        Debug.Log("Height: " + _arCamera.scaledPixelHeight);
        Debug.Log("Width: " + _arCamera.scaledPixelWidth);


    }

    private IEnumerator takeImage()
    {
        yield return new WaitForEndOfFrame();

        cpuImage = new XRCpuImage();
        if (!arCameraManager.TryAcquireLatestCpuImage(out cpuImage))
        {
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


        Debug.Log("Height: " + _arCamera.pixelHeight);
        Debug.Log("Width: " + _arCamera.pixelWidth);



        string fileName = "detect.png";

        string path = Path.Combine(Application.persistentDataPath, fileName);

        byte[] bytes = camTexture.EncodeToPNG();

        File.WriteAllBytes(path, bytes);


        var detectedTexts = webAPI.ocr(path);

        Vector2 centralPoint = CalculateCentroid(detectedTexts[0].Vertices);
        Vector2 normalizedCoordinates = new Vector2(centralPoint.x / Screen.width, centralPoint.y / Screen.height);
        // Convert to world space
        Vector3 worldSpacePoint = _arCamera.ScreenToWorldPoint(new Vector3(normalizedCoordinates.x, normalizedCoordinates.y, 10));


        var (finalX, finalY) = convertCoordinates(image, detectedTexts[0]);


        testText.GetComponent<TextMeshProUGUI>().text = detectedTexts[0].Text + " " + finalX + " " + finalY;

        RectTransform parentCanvas = canvas.GetComponent<RectTransform>();




        var dockerBox = Instantiate(coverPrefab, worldSpacePoint, Quaternion.identity);
        dockerBox.transform.parent = canvas.transform;

        ////string ocrResult = webAPI.imageToText(path);
        //ocrResult = ocrResult.Replace("\\n", " ");
        //ocrResult = ocrResult.Replace("\\f", "");

        //string translation = webAPI.translate(ocrResult);
        //Debug.Log(translation);

        //GameObject translationUI = Instantiate(translationPrefab, new Vector3(0, 0, 0), Quaternion.identity); //Instanting a prefab object

        //translationUI.transform.SetParent(canvas.transform);
        //translationUI.transform.localPosition = new Vector3(1, 0 + 0.5f, 1);
        //GameObject translationText = translationUI.transform.Find("TranslationText").gameObject;
        //translationText.GetComponent<TextMeshProUGUI>().text = translation;
        //translationText.GetComponent<TextMeshProUGUI>().fontSize = 30;

        //startDeleteCo = translationUI.GetComponent<TranslationBox>().startDeleteTimer();
        //StartCoroutine(startDeleteCo);



        //float x = result[1] * cWidth;
        //float y = result[2] * cHeight;
        //float Width = (result[3] - result[1]) * cWidth;
        //float cropWidth = (result[3] - result[1]) * camTexture.width;
        //float Height = (result[2] - result[0]) * cHeight;
        //float cropHeight = (result[2] - result[0]) * camTexture.height;
        //float CenterX = ((result[3] - result[1]) * cWidth) / 2 + (result[1] * cWidth);
        //float CenterY = ((result[2] - result[0]) * cHeight) / 2 + (result[0] * cHeight);


        //byte[] pngImageByteArray = null;

        //pngImageByteArray = File.ReadAllBytes(path);




        //Texture2D tempTexture = new Texture2D(camTexture.width, camTexture.height, format, false);
        //tempTexture.LoadImage(pngImageByteArray);
        //testImage.GetComponent<RawImage>().texture = tempTexture;

        //var dockerBox = Instantiate(coverPrefab);

        //dockerBox.transform.parent = canvas.transform;
        //dockerBox.transform.localPosition = new Vector3(CenterX, CenterY, 2);



    }

    Vector2 CalculateCentroid(List<WebAPI.Vertex> vertices)
    {
        float centerX = 0f;
        float centerY = 0f;
        foreach (var vertex in vertices)
        {
            centerX += vertex.X;
            centerY += vertex.Y;
        }
        centerX /= vertices.Count;
        centerY /= vertices.Count;
        return new Vector2(centerX, centerY);
    }

    public class Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
    }

    public (float x, float y) convertCoordinates(XRCpuImage image, WebAPI.TextItem textItem)
    {

        float topLeftX = textItem.Vertices[0].X;
        float topLeftY = textItem.Vertices[0].Y;

        float botRightX = textItem.Vertices[2].X;
        float botRightY = textItem.Vertices[2].Y;

        float middleX = (botRightX - topLeftX)/2;
        float middleY = (topLeftY - botRightY) / 2;

        float startingX = middleX - (image.width / 2);
        float startingY = middleY - (image.height / 2);

        float xScalar = conversionX / image.width;
        float yScalar = conversionY / image.height;

        float finalX = startingX * xScalar;
        float finalY = startingY * yScalar;

        return (finalX, finalY);

    }

    public List<WebAPI.TextItem> ocrConversion(string imgPath)
    {
        convertedText.setImgPath(imgPath);

        var items = webAPI.ocr(imgPath);

        // convertedText.setConvertedText(items);

        Debug.Log(items.First().Vertices.First().Y.ToString());

        return items;
    }


}
