using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
namespace Qualcomm.Snapdragon.Spaces.Samples
{

    public class WordTracking : MonoBehaviour
    {


        private ARTrackedImageManager arImageManager;

        public XRReferenceImageLibrary library;

        public GameObject[] ArPrefabs;

        public GameObject testText;

        private readonly Dictionary<string,GameObject> instantiatedPrefabs = new Dictionary<string, GameObject>();

        private void Awake()
        {
            arImageManager = GetComponent<ARTrackedImageManager>();

            arImageManager.referenceLibrary = library;

            testText.GetComponent<TextMeshProUGUI>().text = arImageManager.referenceLibrary.count.ToString();

        }

        public void OnEnable()
        {
            arImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            
        }

        public  void OnDisable()
        {
            arImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }


        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args) {
        

            foreach(var trackedImage in args.added)
            {
                var imageName = trackedImage.referenceImage.name;



                foreach (var curPrefab in ArPrefabs)
                {
                    if(string.Compare(curPrefab.name, imageName, StringComparison.OrdinalIgnoreCase) == 0
                        && !instantiatedPrefabs.ContainsKey(imageName))
                    {
                        var newPrefab = Instantiate(curPrefab, trackedImage.transform);

                        instantiatedPrefabs[imageName] = newPrefab;
                    }
                }
            }

            foreach (var trackedImage in args.updated)
            {
                instantiatedPrefabs[trackedImage.referenceImage.name]
                    .SetActive(trackedImage.trackingState == TrackingState.Tracking);
            }

            foreach(var trackedImage in args.removed)
            {
                Destroy(instantiatedPrefabs[trackedImage.referenceImage.name]);

                instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
            }

        }
    }

}