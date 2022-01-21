//-----------------------------------------------------------------------
// <copyright file="DetectedPlaneGenerator.cs" company="Google LLC">
//
// Copyright 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.Common
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;

    /// <summary>
    /// Manages the visualization of detected planes in the scene.
    /// </summary>
    public class DetectedPlaneGenerator : MonoBehaviour
    {
        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is
        /// used across the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> _newPlanes = new List<DetectedPlane>();
        public List<DetectedPlaneVisualizer> _allPlanes = new List<DetectedPlaneVisualizer>();

        public bool planesAreVisible;

        public void Update()
        {
            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to
            // visualize them.
            Session.GetTrackables<DetectedPlane>(_newPlanes, TrackableQueryFilter.New);
            for (int i = 0; i < _newPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The
                // transform is set to the origin with an identity rotation since the mesh for our
                // prefab is updated in Unity World coordinates.

                // If planes are never to be visible, their color is clear
                var color = (ArManager.Instance.planesVisibility == PlanesVisibility.AlwaysHide) ? Color.clear : ArManager.Instance.planeColor;

                GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                var ojectVisualizer = planeObject.GetComponent<DetectedPlaneVisualizer>();
                ojectVisualizer.Initialize(_newPlanes[i], color, ArManager.Instance.planeTexture);
                _allPlanes.Add(ojectVisualizer);
            }

            // Get all instantiated planes to set their visibility
            for (int i = 0; i < _allPlanes.Count; i++)
            {
                if(_allPlanes[i] != null)
                {
                    _allPlanes[i].GetComponent<MeshRenderer>().enabled = planesAreVisible;
                }
            }
        }

        public void SetVisibility(bool visible)
        {
            // If planes are always meant to be invisible, "planesAreVisible" is initialized as false, and is never changed
            if (ArManager.Instance.planesVisibility == PlanesVisibility.AlwaysHide)
                return;

            planesAreVisible = visible;
        }
    }
}