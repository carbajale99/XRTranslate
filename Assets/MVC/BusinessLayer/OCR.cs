using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OCR : MonoBehaviour
{
    private WebAPI webAPI = new WebAPI();
    private ConvertedText convertedText = new ConvertedText();

    public ConvertedText ocr(string imgPath)
    {
        convertedText.setImgPath(imgPath);

        string conversion = webAPI.imageToText(imgPath);

        convertedText.setConvertedText(conversion);

        return convertedText;
    }


}
