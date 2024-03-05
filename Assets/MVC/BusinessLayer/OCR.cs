using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

public class OCR : MonoBehaviour
{
    private WebAPI webAPI = new WebAPI();

    private ConvertedText convertedText = new ConvertedText();

    private string imgPath = "C:\\Users\\tuant\\Downloads\\IMG_0827.JPG";

    public void ocrClick()
    {
        var items = ocrConversion();
        //Debug.Log(items);

    }

    public List<WebAPI.TextItem> ocrConversion()
    {
        convertedText.setImgPath(imgPath);

        var items = webAPI.ocr(imgPath);

        // convertedText.setConvertedText(items);

        Debug.Log(items.First().Vertices.First().Y.ToString());

        return items;
    }


}
