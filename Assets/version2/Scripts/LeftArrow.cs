using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArrow : MonoBehaviour
{

    public GameObject wordsUI;

    private GameObject currentWordContainer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void backward()
    {
        if(wordsUI.transform.childCount == 0)
        {
            return;
        }

        foreach(Transform wordContainer in wordsUI.transform)
        {
            if(wordContainer.gameObject.activeInHierarchy)
            {
                currentWordContainer = wordContainer.gameObject;
            }
        }

        int nextChildIndex = currentWordContainer.transform.GetSiblingIndex() - 1;
        if(nextChildIndex == -1)
        {
            nextChildIndex = wordsUI.transform.childCount-1;
        }
        currentWordContainer.SetActive(false);
       
        wordsUI.transform.GetChild(nextChildIndex).gameObject.SetActive(true);
        //Debug.Log(currentWordContainer.transform.GetSiblingIndex());

    }
}
