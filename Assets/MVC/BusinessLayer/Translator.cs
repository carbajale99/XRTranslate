using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translator : MonoBehaviour
{

    private WebAPI webAPI = new WebAPI();

    public string translate(string phrase)
    {
        string translationURL = stringToTranslationPar(phrase);
        return webAPI.translate(translationURL);
    }

    private string stringToTranslationPar(string phrase) //function to set up parameter 
    {
        string baseURL = "/translator?phrase="; //base URL

        string finalURL = baseURL + phrase; //baseURL + phrase to translate

        return finalURL;
    }
}
