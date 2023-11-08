using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnchorSimulate : MonoBehaviour

{
    public int xPos = 0;
    public int yPos = 0;
    int screenWidth = Screen.width;
    int screenHeight = Screen.height;
    TextMeshProUGUI anchorTest;
    float timer = 0.0f;
    float waitTime = 2.0f;
    public bool isDebug = false;

    // Start is called before the first frame update
    void Start()
    {
        anchorTest = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > waitTime)
        {
            xPos++;
            yPos++;
            anchorTest.text = "x = " + xPos.ToString() + "y = " + yPos.ToString();
            timer = 0.0f;
        }
        else
        {
            timer = timer + Time.deltaTime;
            if (isDebug)
            {
                Debug.Log(timer);
            }
        
        }

    }
}
