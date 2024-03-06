using Qualcomm.Snapdragon.Spaces;
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

public class MLWordDetector : MonoBehaviour
{

    private Transform _arCameraTransform;
    private Camera _arCamera;

    private ARCameraManager arCameraManager;


    private Texture2D camTexture;
    private XRCpuImage cpuImage;

    public GameObject coverPrefab;
    public GameObject canvas;

    public GameObject testText;
    public GameObject testImage;

    public GameObject translationPrefab;

    private IEnumerator startDeleteCo;

    private float conversionX = 3.9f;

    private float conversionY = 2.1f;

    private WebAPI webAPI;
    // Start is called before the first frame update
    void Start()
    {
        _arCamera = OriginLocationUtility.GetOriginCamera();
        _arCameraTransform = _arCamera.transform;

        arCameraManager = _arCamera.GetComponent<ARCameraManager>();

        webAPI = new WebAPI();
    }


    
    //public void detect()
    //{
    //    string tempPath = "C:\\Users\\edgar\\Downloads\\tuanmand.JPG";

    //    float[] result = webAPI.detect(tempPath);

    //    float x = result[1] * imgWidth;
    //    float y = result[2] * imgHeight;
    //    float Width = (result[3] - result[1]) * imgWidth;
    //    float Height = (result[2] - result[0]) * imgHeight;
    //    float CenterX = ((result[3] - result[1]) * imgWidth) / 2 + (result[1] * imgWidth);
    //    float CenterY = ((result[2] - result[0]) * imgHeight) / 2 + (result[0] * imgHeight);
    //}
    public void detect()
    {

        StartCoroutine(detectImage());

    }

    private IEnumerator detectImage()
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

        float extractedX = 0;
        float extractedY = 0;

        float startingX = extractedX - (image.width/2);
        float startingY = extractedY - (image.height/2);

        float xScalar = conversionX/image.width;
        float yScalar = conversionY/image.height;

        float finalX = startingX * xScalar;
        float finalY = startingY * yScalar;


        string fileName = "detect.png";

        string path = Path.Combine(Application.persistentDataPath, fileName);

        byte[] bytes = camTexture.EncodeToPNG();

        File.WriteAllBytes(path, bytes);


        

        //deals with tensor flow api
        //float[] result = webAPI.detect(path);

        //for(int i = 0; i < result.Length; i++)
        //{
        //    result[i] -= (float).5;
        //}

        //RectTransform parentCanvas = canvas.GetComponent<RectTransform>();
        //Vector3 canvasWidthHeight = new Vector3(result[1] , result[0] , 2);

        //var dockerBox = Instantiate(coverPrefab, canvasWidthHeight, Quaternion.identity);
        //dockerBox.transform.parent = canvas.transform;

        string ocrResult = webAPI.imageToText(path);
        ocrResult = ocrResult.Replace("\\n", " ");
        ocrResult = ocrResult.Replace("\\f", "");

        string translation = webAPI.translate(ocrResult);
        Debug.Log(translation);

        GameObject translationUI = Instantiate(translationPrefab, new Vector3(0, 0, 0), Quaternion.identity); //Instanting a prefab object

        translationUI.transform.SetParent(canvas.transform);
        translationUI.transform.localPosition = new Vector3(1, 0 + 0.5f, 1);
        GameObject translationText = translationUI.transform.Find("TranslationText").gameObject;
        translationText.GetComponent<TextMeshProUGUI>().text = translation;
        translationText.GetComponent<TextMeshProUGUI>().fontSize = 30;

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

        testText.GetComponent<TextMeshProUGUI>().text = translation;

        byte[] pngImageByteArray = null;

        pngImageByteArray = File.ReadAllBytes(path);


        

        Texture2D tempTexture = new Texture2D(camTexture.width, camTexture.height, format, false);
        tempTexture.LoadImage(pngImageByteArray);
        testImage.GetComponent<RawImage>().texture = tempTexture;

        //var dockerBox = Instantiate(coverPrefab);

        //dockerBox.transform.parent = canvas.transform;
        //dockerBox.transform.localPosition = new Vector3(CenterX, CenterY, 2);



    }
}
