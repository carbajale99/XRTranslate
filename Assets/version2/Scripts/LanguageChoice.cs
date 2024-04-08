using Qualcomm.Snapdragon.Spaces.Samples;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageChoice : MonoBehaviour
{

    public GameObject chosenLanguage;
    public GameObject buttonText;
    public GameObject mainCanvas;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

      
    public void choseLanguage()
    {
        chosenLanguage.GetComponent<TextMeshProUGUI>().text = buttonText.GetComponent<TextMeshProUGUI>().text;

        chosenLanguage.transform.parent.gameObject.SetActive(false);

        mainCanvas.SetActive(true);
        mainCanvas.GetComponent<FloatingPanelController>().FollowGaze = true;
    }
}
