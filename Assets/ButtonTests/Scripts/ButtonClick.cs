using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonClick : MonoBehaviour
{
    // Start is called before the first frame update
    //variables:
    public Button ourButton;
    public GameObject translationUIPrefab;
    private Vector3 buttonPosition;

    void Start()
    {

        buttonPosition = ourButton.transform.position; //getting the position of the button

        Debug.Log("button pos: " + ourButton.transform.position);
    }  

    //what to do when button is clicked
    public void buttonClicked()
    {
        GameObject translation = Instantiate(translationUIPrefab, new Vector3(0,0,0), Quaternion.identity); //Instanting a prefab object

        translation.transform.SetParent(ourButton.transform);
        translation.transform.localScale = Vector3.one;
        translation.transform.localPosition = new Vector3(buttonPosition.x, buttonPosition.y + 200.0f, buttonPosition.z);
        GameObject translationText = translation.transform.Find("TranslationText").gameObject;
        translationText.GetComponent<TextMeshProUGUI>().text = "Less goo";

        Debug.Log("Clicked!");



    } 
}
