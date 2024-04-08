using Qualcomm.Snapdragon.Spaces.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear : MonoBehaviour
{
    public GameObject optionsUI;

    public GameObject wordsUI;

    public GameObject mainCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void clear()
    {
        if (wordsUI.transform.childCount != 0)
        {
            foreach (Transform child in wordsUI.transform) { 
                
                Destroy(child.gameObject);

            }
        }

        optionsUI.SetActive(false);
        mainCanvas.GetComponent<FloatingPanelController>().FollowGaze = true;
    }
}
