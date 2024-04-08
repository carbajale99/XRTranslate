using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeButton : MonoBehaviour
{

    public GameObject mainCanvas;
    public GameObject languageCanvas;
    public GameObject optionsUI;
    public GameObject wordsUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void restart() {

        if (wordsUI.transform.childCount != 0)
        {
            foreach (Transform child in wordsUI.transform)
            {
                foreach(Transform gChild in child.transform)
                {

                    Destroy(gChild.gameObject);
                }


            }
        }
        mainCanvas.SetActive(false);
        optionsUI.SetActive(false);
        languageCanvas.SetActive(true);

    }
}
