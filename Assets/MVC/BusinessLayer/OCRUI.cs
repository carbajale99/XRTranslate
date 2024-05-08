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
using Qualcomm.Snapdragon.Spaces.Samples;

public class OCRUI : MonoBehaviour
{
    private WebAPI webAPI = new WebAPI();

    private ConvertedText convertedText = new ConvertedText();

    private XRCpuImage cpuImage;
    private Texture2D camTexture;

    private Transform _arCameraTransform;
    private Camera _arCamera;

    private ARCameraManager arCameraManager;

    public GameObject testText;

    public GameObject translationButtonPrefab;
    public GameObject canvas;

    private string testImage = "C:\\Users\\edgar\\Downloads\\words.JPG";

    public GameObject wordsUI;

    public GameObject wordsContainerPrefab;

    public GameObject optionsUI;

    void Start()
    {
        _arCamera = OriginLocationUtility.GetOriginCamera();
        _arCameraTransform = _arCamera.transform;

        arCameraManager = _arCamera.GetComponent<ARCameraManager>();

        webAPI = new WebAPI();
    }

    public void ocrClick()
    {

        //StartCoroutine(takeImage());


        var detectedTexts = webAPI.ocr(testImage);


        int translationButtonCount = 3;

        int startingWord = 0;

        int wordContainerCount = (int)Math.Ceiling((double)detectedTexts.Count / (double)3);



        for (int i = 0; i < wordContainerCount; i++)
        {
            float yPosition = 150f;

            GameObject wordContainer = Instantiate(wordsContainerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            wordContainer.transform.SetParent(wordsUI.transform);
            wordContainer.transform.localScale = Vector3.one;
            wordContainer.transform.localPosition = new Vector3(0, 0, 0);

            if (i != 0)
            {
                wordContainer.SetActive(false);
            }


            if (translationButtonCount > detectedTexts.Count - startingWord)
            {
                translationButtonCount = detectedTexts.Count - startingWord;
            }

            for (int j = 0; j < translationButtonCount; j++)
            {
                Debug.Log(detectedTexts[startingWord].Text);
                GameObject translationButton = Instantiate(translationButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);

                translationButton.transform.SetParent(wordContainer.transform);
                translationButton.transform.localScale = Vector3.one;
                translationButton.transform.localPosition = new Vector3(0, yPosition, 1);
                GameObject translationButtonText = translationButton.transform.Find("ButtonText").gameObject;
                translationButtonText.GetComponent<TextMeshProUGUI>().text = detectedTexts[startingWord].Text;

                startingWord++;

                yPosition -= 150;

            }

        }

        optionsUI.SetActive(true);



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


        int translationButtonCount = 3;

        int startingWord = 0;

        int wordContainerCount = (int)Math.Ceiling((double)detectedTexts.Count / (double)3);



        for (int i = 0; i < wordContainerCount; i++)
        {
            float yPosition = 150f;

            GameObject wordContainer = Instantiate(wordsContainerPrefab, new Vector3(0, 0, 0), canvas.transform.rotation);
            wordContainer.transform.SetParent(wordsUI.transform);
            wordContainer.transform.localScale = Vector3.one;
            wordContainer.transform.localPosition = new Vector3(0, 0, 0);

            if (i != 0)
            {
                wordContainer.SetActive(false);
            }


            if (translationButtonCount > detectedTexts.Count - startingWord)
            {
                translationButtonCount = detectedTexts.Count - startingWord;
            }

            for (int j = 0; j < translationButtonCount; j++)
            {
                //Debug.Log(detectedTexts[startingWord].Text);
                GameObject translationButton = Instantiate(translationButtonPrefab, new Vector3(0, 0, 0), canvas.transform.rotation);

                translationButton.transform.SetParent(wordContainer.transform);
                translationButton.transform.localScale = Vector3.one;
                translationButton.transform.localPosition = new Vector3(0, yPosition, 1);
                GameObject translationButtonText = translationButton.transform.Find("ButtonText").gameObject;
                translationButtonText.GetComponent<TextMeshProUGUI>().text = detectedTexts[startingWord].Text;

                startingWord++;

                yPosition -= 150;

            }

        }

        optionsUI.SetActive(true);
        canvas.GetComponent<FloatingPanelController>().FollowGaze = false;
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
