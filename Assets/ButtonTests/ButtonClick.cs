using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonClick : MonoBehaviour
{
    // Start is called before the first frame update
    //variables:
    public Canvas ourCanvas;
    public Button ourButton;
    public GameObject translationUIPrefab;
    private Vector3 buttonPosition;

    void Start()
    {
        //canvas = GetComponent<Canvas>(); //get the canvas
        //ourButton = GameObject.Find("ButtonTest").GetComponent<Button>(); //get the button
        //newText = GameObject.Find("TextTest").GetComponent<TextMeshProUGUI>();
        //ourCanvas = GameObject.Find("TestCanvas").GetComponent<Canvas>();


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


        //GameObject gO = new GameObject();

        //TextMeshProUGUI newText = gO.AddComponent<TextMeshProUGUI>();


        //newText.text = "Les gooo"; //making a text and font
        //newText.fontSize = 30.0f;

        //float goX = gO.transform.GetComponent<RectTransform>().localScale.x;
        //float goY = gO.transform.GetComponent<RectTransform>().localScale.y;
        //float goZ = gO.transform.GetComponent<RectTransform>().localScale.z;


        //gO.transform.SetParent(ourButton.transform);

        //gO.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //gO.transform.localPosition = new Vector3(buttonPosition.x +30.0f, buttonPosition.y + 150.0f, buttonPosition.z); //text is above the button
        //Debug.Log("text posk: " + gO.transform.position + " text: " + newText.text);

    } 
}
