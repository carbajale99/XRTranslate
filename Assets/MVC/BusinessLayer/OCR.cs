using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OCR : MonoBehaviour
{
    private WebAPI webAPI = new WebAPI();
    private ConvertedText convertedText;

    public string ocr(string imgPath)
    {
        return webAPI.imageToText(imgPath);
    }


}
