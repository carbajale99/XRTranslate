using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveHiglight : MonoBehaviour
{
    //    public GameObject anchorText;
    //    AnchorSimulate anchorSimScript

    //public variables to work with
    public GameObject cube; 
    Vector3 position;
    TextMeshProUGUI highlightText;

    // Start is called before the first frame update
    void Start()
    {
        highlightText = GetComponent<TextMeshProUGUI>();
        cube = GameObject.Find("AnchorCube"); //gets the cube from unity to code
        //position = cube.transform.position; //position of the cube
        //highlightText.transform.position = new Vector3(position.x, position.y+2.0f, position.z); //get highlight text to position above cube (up 20)

    }

    // Update is called once per frame
    void Update()
    {
        
        cube.transform.position += new Vector3(0.01f, 0, 0); //move cube across the x axis 2
        position = cube.transform.position; //position of the cube
        highlightText.transform.position = new Vector3(cube.transform.position.x, cube.transform.position.y+2.0f, cube.transform.position.z); //get highlight text to position above cube (up 20)
        Debug.Log(position.x);
    }
}
