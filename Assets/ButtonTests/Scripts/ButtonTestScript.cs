/******************************************************************************
 * File: ButtonTestScript.cs
 * Copyright (c) 2022-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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

        public Button ourButton;
        public GameObject translationUIPrefab;
        private Vector3 buttonPosition;

        public void buttonClicked()
        {
            if(ourButton.transform.childCount > 1)
            {
                GameObject.Destroy(ourButton.transform.Find("TranslationUI(Clone)").gameObject);
            }
            else
            {
                GameObject translation = Instantiate(translationUIPrefab, new Vector3(0, 0, 0), Quaternion.identity); //Instanting a prefab object

                translation.transform.SetParent(ourButton.transform);
                translation.transform.localScale = Vector3.one;
                translation.transform.localPosition = new Vector3(buttonPosition.x, buttonPosition.y + 200.0f, buttonPosition.z);
                GameObject translationText = translation.transform.Find("TranslationText").gameObject;
                translationText.GetComponent<TextMeshProUGUI>().text = "Less goo";

                Debug.Log("Clicked!");
            }
        }

        private void Start()
        {
            _arCamera = OriginLocationUtility.GetOriginCamera();
            _arCameraTransform = _arCamera.transform;
            _interactionManager ??= FindObjectOfType<InteractionManager>(true);

            buttonPosition = ourButton.transform.position;
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
