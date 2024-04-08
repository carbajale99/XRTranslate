using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translator : MonoBehaviour
{

    private WebAPI webAPI = new WebAPI();
    private Translation translation = new Translation();

    public Translation translate(string phrase, string language)
    {
        translation.setPhrase(phrase);
        string translationURL = stringToTranslationPar(translation.getPhrase());

        string newPhrase = webAPI.translate(translationURL, language);
        translation.setTranslation(newPhrase);

        return translation;

    }

    private string stringToTranslationPar(string phrase) //function to set up parameter 
    {
        string baseURL = "/translator?phrase="; //base URL

        string finalURL = baseURL + phrase; //baseURL + phrase to translate

        return finalURL;
    }
}
