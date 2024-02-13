/******************************************************************************
 * File: SpatialMeshingSampleController.cs
 * Copyright (c) 2022-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class SpatialMeshingSampleController : SampleController
    {
        public Toggle CalculateCpuNormalsToggle;
        public Toggle UseSmoothedNormalsToggle;
        public Text MeshOpacityValueText;
        public Scrollbar OpacityScrollBar;

        public MeshFilter CustomShaderPrefab;
        public MeshFilter CpuNormalsPrefab;
        private ARMeshManager _meshManager;
        private SpacesARMeshManagerConfig _meshManagerConfig;

        public void Awake()
        {
            _meshManager = FindObjectOfType<ARMeshManager>();
            _meshManagerConfig = FindObjectOfType<SpacesARMeshManagerConfig>();

            if (_meshManager == null)
            {
                Debug.LogError("Could not find mesh manager. Sample will not work correctly.");
            }
        }

        public override void Start()
        {
            base.Start();

            bool canEnableCpuNormals = _meshManagerConfig != null && _meshManager.normals;
            if (!canEnableCpuNormals)
            {
                Debug.LogWarning("Cannot sensibly enable cpu normals for this sample. Use CPU Normals checkbox will be disabled.");
                CalculateCpuNormalsToggle.isOn = false;
                CalculateCpuNormalsToggle.interactable = false;
                UseSmoothedNormalsToggle.SetIsOnWithoutNotify(false);
                UseSmoothedNormalsToggle.interactable = false;
            }
            else
            {
                CalculateCpuNormalsToggle.isOn = _meshManagerConfig.CalculateCpuNormals;
                UseSmoothedNormalsToggle.isOn = _meshManagerConfig.UseSmoothedNormals;
            }

            SwitchPrefab(CalculateCpuNormalsToggle.isOn);
            UseSmoothedNormalsToggle.gameObject.SetActive(CalculateCpuNormalsToggle.isOn);
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (!GetSubsystemCheck())
            {
                return;
            }

            _meshManager.meshesChanged += OnMeshesChanged;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _meshManager.meshesChanged -= OnMeshesChanged;
        }

        private void OnMeshesChanged(ARMeshesChangedEventArgs args)
        {
            foreach (var meshFilter in args.added)
            {
                Debug.Log("Added meshFilter: " + meshFilter.name);
            }

            foreach (var meshFilter in args.updated)
            {
                Debug.Log("Updated meshFilter: " + meshFilter.name);
            }

            foreach (var meshFilter in args.removed)
            {
                Debug.Log("Removed meshFilter: " + meshFilter.name);
            }

            UpdateMeshOpacity(OpacityScrollBar.value);
        }

        protected override bool CheckSubsystem()
        {
            if (_baseRuntimeFeature.CheckServicesCameraPermissions())
            {
                return _meshManager.subsystem?.running ?? false;
            }

            Debug.LogError("Snapdragon Spaces Services has no camera permissions! Spatial Meshing feature disabled.");
            return false;
        }

        public void ToggleCalculateCpuNormals(bool cpuNormalsEnabled)
        {
            if (_meshManagerConfig != null)
            {
                _meshManagerConfig.CalculateCpuNormals = cpuNormalsEnabled;
            }

            UseSmoothedNormalsToggle.gameObject.SetActive(cpuNormalsEnabled);

            if (_meshManager != null)
            {
                var meshes = _meshManager.meshes;
                // Need to destroy the MeshFilters because we want the MeshManager to regenerate them with a new prefab.
                foreach (var mesh in meshes)
                {
                    Destroy(mesh);
                }

                SwitchPrefab(cpuNormalsEnabled);
            }
        }

        public void ToggleUseSmoothedNormals(bool smoothedNormalsEnabled)
        {
            if (_meshManagerConfig != null)
            {
                _meshManagerConfig.UseSmoothedNormals = smoothedNormalsEnabled;
            }
        }

        public void OnScrollValueChanged(float value)
        {
            SendHapticImpulse(duration: 0.1f);
            UpdateMeshOpacity(value);
        }

        private void UpdateMeshOpacity(float value)
        {
            // Get the meshes from the Mesh Manager
            var meshes = _meshManager.meshes;
            if (meshes == null)
            {
                Debug.LogWarning("No meshes generated yet to change the color.");
                return;
            }
            // Change the alpha in the meshes materials.
            foreach (var mesh in meshes)
            {
                var materialColor = mesh.gameObject.GetComponent<Renderer>().material.color;
                var newAlpha= Math.Clamp(value, 0.1f, 1f);
                materialColor.a = newAlpha;
                MeshOpacityValueText.text = newAlpha.ToString("#0.00");
                mesh.gameObject.GetComponent<Renderer>().material.color = materialColor;
            }
        }

        private void SwitchPrefab(bool cpuNormalsEnabled)
        {
            if (cpuNormalsEnabled && _meshManager.normals)
            {
                // Don't switch to cpu normals prefab unless the normals are enabled on the MeshManager
                _meshManager.meshPrefab = CpuNormalsPrefab;
            }
            else
            {
                _meshManager.meshPrefab = CustomShaderPrefab;
            }
        }
    }
}
