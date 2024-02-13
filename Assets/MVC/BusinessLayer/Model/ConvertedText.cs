using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertedText : MonoBehaviour
{
    private string imgPath;
    private string convertedText;

    public string getImgPath() {  return imgPath; }
    public void setImgPath(string ip) { imgPath = ip; }
    public string getConvertedText() {  return convertedText; }
    public void setConvertedText(string ct) { convertedText = ct; }

}
