using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PositionListener : MonoBehaviour
{

    public bool positionGiven = false;
    public GameObject translationUIPrefab;
    public Vector3 buttonPosition;
    public Canvas ourCanvas;
    public string ourText;

    private IEnumerator startDeleteCo;

    // Start is called before the first frame update
    void Start()
    {
        buttonPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (positionGiven)
        {

            GameObject translation = Instantiate(translationUIPrefab, new Vector3(0, 0, 0), Quaternion.identity); //Instanting a prefab object

            translation.transform.SetParent(ourCanvas.transform);
            //translation.transform.localScale = Vector3.one;
            translation.transform.localPosition = new Vector3(buttonPosition.x, buttonPosition.y + 0.5f, buttonPosition.z);
            GameObject translationText = translation.transform.Find("TranslationText").gameObject;
            translationText.GetComponent<TextMeshProUGUI>().text = ourText;
            translationText.GetComponent<TextMeshProUGUI>().fontSize = 30;
            Debug.Log(translation.name + " $$$");
            startDeleteCo = translation.GetComponent<TranslationBox>().startDeleteTimer();
            StartCoroutine(startDeleteCo);
            Debug.Log("corout started");


            Debug.Log("Clicked!");

            positionGiven = false;
        }
    }
      
}