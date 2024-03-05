/******************************************************************************
 * File: ButtonTestScript.cs
 * Copyright (c) 2022-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************//*

using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
//using UnityEditor.PackageManager;
using System.Net.Http;
using System.Collections;
using System.IO;
//using System.Text;
//using System.Text.Json;
//using Unity.VisualScripting;
//using System;
//using System.Linq;
//using System.Web;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class ButtonTestScript : MonoBehaviour
    {
        [Tooltip("Whether the Game Object should follow or not the camera movement.")]
        public bool FollowGaze = true;
        [Tooltip("Target distance between the camera and the Game Object.")]
        public float TargetDistance = 1.0f;
        [Tooltip("Smoothness on the movement following the Game Object.")]
        public float MovementSmoothness = 0.2f;
        [Tooltip("Vertical follow threshold.")]
        public float VerticalBias = 0.8f;
        [Tooltip("Horizontal follow threshold.")]
        public float HorizontalBias = 0.5f;
        private Transform _arCameraTransform;
        private Camera _arCamera;
        private InteractionManager _interactionManager;

        //public GameObject ourContainer;
        public Canvas ourCanvas; 
        public Button ourButton;
        public GameObject translationUIPrefab;
        private Vector3 buttonPosition;


        private HttpClient client;

        private WebAPI webAPI;

        private string testImage = "C:/Users/edgar/Downloads/hello.png";

        private PositionListener positionListener;

        //public void buttonClicked()
        //{

        //    positionListener.buttonPosition = buttonPosition;
        //    positionListener.positionGiven = true;


        //    GameObject buttonTextObj = ourButton.transform.Find("ButtonText").gameObject; //getting buttonText 
        //    string buttonText = buttonTextObj.GetComponent<TextMeshProUGUI>().text; //converting game obj to string

        //    string finalResult = await webAPI.imageToTextAsync(testImage);

        //    Debug.Log

        //    string translationurl = stringtotranslationpar(buttontext); //translating text

        //    var response = client.getasync(translationurl).result; //response from api


        //    if (response.issuccessstatuscode) //if translation success display
        //    {
        //        var responsecontent = response.content.readasstringasync().result;
        //        positionlistener.ourtext = responsecontent.replace("\"", "");
        //        debug.log(responsecontent);
        //    }
        //    else //else error
        //    {
        //        debug.log("error: " + response.statuscode);
        //    }
        //}

        public void buttonClicked()
        {

            positionListener.buttonPosition = buttonPosition;
            positionListener.positionGiven = true;


            GameObject buttonTextObj = ourButton.transform.Find("ButtonText").gameObject; //getting buttonText 
            string buttonText = buttonTextObj.GetComponent<TextMeshProUGUI>().text; //converting game obj to string

            var finalResult = webAPI.imageToText(testImage);

            Debug.Log(finalResult);

        }



        private void Start()
        {
            _arCamera = OriginLocationUtility.GetOriginCamera();
            _arCameraTransform = _arCamera.transform;
            _interactionManager ??= FindObjectOfType<InteractionManager>(true);
            
            buttonPosition = ourButton.transform.position;//getting the position of the button

            positionListener = ourCanvas.GetComponent<PositionListener>();

            webAPI = new WebAPI();

            
        
        }

        private string stringToTranslationPar(string phrase) //function to set up parameter 
        {   
            string baseURL = "/translator?phrase="; //base URL

            string finalURL = baseURL + phrase; //baseURL + phrase to translate

            return finalURL;
        }

        private void Update()
        {
            if (FollowGaze)
            {
                AdjustPanelPosition();
            }
        }


        // Adjusts the position of the Panel if the gaze moves outside of the inner rectangle of the FOV,
        //  which is half the length in both axis.
        private void AdjustPanelPosition()
        {
            var headPosition = _arCameraTransform.position;
            var gazeDirection = _arCameraTransform.forward;
            var direction = (transform.position - headPosition).normalized;
            var targetPosition = headPosition + (gazeDirection * TargetDistance);
            var targetDirection = (targetPosition - headPosition).normalized;
            var eulerAngles = Quaternion.LookRotation(direction).eulerAngles;
            var targetEulerAngles = Quaternion.LookRotation(targetDirection).eulerAngles;
            var verticalHalfAngle = _arCamera.fieldOfView * VerticalBias;
            eulerAngles.x += GetAdjustedDelta(targetEulerAngles.x - eulerAngles.x, verticalHalfAngle);
            var horizontalHalfAngle = _arCamera.fieldOfView * HorizontalBias * _arCamera.aspect;
            eulerAngles.y += GetAdjustedDelta(targetEulerAngles.y - eulerAngles.y, horizontalHalfAngle);
            targetPosition = headPosition + (Quaternion.Euler(eulerAngles) * Vector3.forward * TargetDistance);
            transform.position = Vector3.Lerp(transform.position, targetPosition, MovementSmoothness);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - headPosition), MovementSmoothness);
        }

        // Returns the normalized delta to a certain threshold, if it exceeds that threshold. Otherwise return 0.
        private float GetAdjustedDelta(float angle, float threshold)
        {
            // Normalize angle to be between 0 and 360.
            angle = ((540f + angle) % 360f) - 180f;
            if (Mathf.Abs(angle) > threshold)
            {
                return -angle / Mathf.Abs(angle) * (threshold - Mathf.Abs(angle));
            }

            return 0f;
        }
    }
}
*/