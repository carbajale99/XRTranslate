using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TranslationButtonClick : MonoBehaviour
{
    public Button ourButton;
    public GameObject translationUIPrefab;
    public GameObject chosenLanguage;
    private Vector3 buttonPosition;

    private WebAPI webAPI = new WebAPI();



    // Start is called before the first frame update
    void Start()
    {
        buttonPosition = ourButton.transform.position; //getting the position of the button

        webAPI = new WebAPI();
    }

    public void createTranslation()
    {
        if(ourButton.transform.childCount > 1)
        {
            Destroy(ourButton.transform.GetChild(1).gameObject);
        }


        GameObject translation = Instantiate(translationUIPrefab, new Vector3(0, 0, 0), ourButton.transform.rotation); //Instanting a prefab object

        translation.transform.SetParent(ourButton.transform);
        translation.transform.localScale = Vector3.one;
        translation.transform.localPosition = new Vector3(-250, 0, 0);
        GameObject buttonText = ourButton.transform.Find("ButtonText").gameObject;
        string textToTranslate = buttonText.GetComponent<TextMeshProUGUI>().text;

        GameObject translationText = translation.transform.Find("TranslationText").gameObject;
        string language = ourButton.transform.parent.parent.parent.parent.parent.transform.Find("LanguagesCanvas").gameObject.transform.Find("ChosenLanguage").gameObject.GetComponent<TextMeshProUGUI>().text;

        string translatedText = webAPI.translate(textToTranslate,language);



        translationText.GetComponent<TextMeshProUGUI>().text = translatedText;



    }
}
