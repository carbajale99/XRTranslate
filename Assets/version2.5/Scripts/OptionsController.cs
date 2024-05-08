using Qualcomm.Snapdragon.Spaces.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject languageCanvas;
    public GameObject optionsUI;
    public GameObject wordsUI;
    private GameObject currentWordContainer;



    public void home()
    {
        if (wordsUI.transform.childCount != 0)
        {
            foreach (Transform child in wordsUI.transform)
            {
                foreach (Transform gChild in child.transform)
                {

                    Destroy(gChild.gameObject);
                }


            }
        }
        mainCanvas.SetActive(false);
        optionsUI.SetActive(false);
        languageCanvas.SetActive(true);
    }

    public void clear()
    {
        if (wordsUI.transform.childCount != 0)
        {
            foreach (Transform child in wordsUI.transform)
            {

                Destroy(child.gameObject);

            }
        }

        optionsUI.SetActive(false);
        mainCanvas.GetComponent<FloatingPanelController>().FollowGaze = true;
    }

    public void backward()
    {
        if (wordsUI.transform.childCount == 0)
        {
            return;
        }

        foreach (Transform wordContainer in wordsUI.transform)
        {
            if (wordContainer.gameObject.activeInHierarchy)
            {
                currentWordContainer = wordContainer.gameObject;
            }
        }

        int nextChildIndex = currentWordContainer.transform.GetSiblingIndex() - 1;
        if (nextChildIndex == -1)
        {
            nextChildIndex = wordsUI.transform.childCount - 1;
        }
        currentWordContainer.SetActive(false);

        wordsUI.transform.GetChild(nextChildIndex).gameObject.SetActive(true);
    }

    public void forward()
    {
        if (wordsUI.transform.childCount == 0)
        {
            return;
        }

        foreach (Transform wordContainer in wordsUI.transform)
        {
            if (wordContainer.gameObject.activeInHierarchy)
            {
                currentWordContainer = wordContainer.gameObject;
            }
        }

        int nextChildIndex = currentWordContainer.transform.GetSiblingIndex() + 1;
        if (nextChildIndex == wordsUI.transform.childCount)
        {
            nextChildIndex = 0;
        }
        currentWordContainer.SetActive(false);

        wordsUI.transform.GetChild(nextChildIndex).gameObject.SetActive(true);
    }
}
