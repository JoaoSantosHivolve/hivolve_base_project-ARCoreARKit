//-----------------------------------------------------------------------
// <copyright file="ARCoreWorldOriginHelper.cs" company="Google LLC">
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

namespace GoogleARCore.Examples.CloudAnchors
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;

    /// <summary>
    /// A helper script to set the apparent world origin of ARCore through applying an offset to the
    /// ARCoreDevice (and therefore also it's FirstPersonCamera child); this also provides
    /// mechanisms to handle resulting changes to ARCore plane positioning and raycasting.
    /// </summary>
    public class ARCoreWorldOriginHelper : MonoBehaviour
    {
        /// <summary>
        /// The transform of the ARCore Device.
        /// </summary>
        public Transform ARCoreDeviceTransform;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is
        /// used across the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> _newPlanes = new List<DetectedPlane>();

        /// <summary>
        /// A list to hold the planes ARCore began tracking before the WorldOrigin was placed.
        /// </summary>
        private List<GameObject> _planesBeforeOrigin = new List<GameObject>();

        /// <summary>
        /// Indicates whether the Origin of the new World Coordinate System, i.e. the Cloud Anchor,
        /// was placed.
        /// </summary>
        private bool _isOriginPlaced = false;

        /// <summary>
        /// The Transform of the Anchor object representing the World Origin.
        /// </summary>
        private Transform _anchorTransform;


        private AppManager m_Manager;

        private void Awake()
        {
            m_Manager = AppManager.Instance;
        }

        public void Update()
        {
            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            Pose worldPose = WorldToAnchorPose(Pose.identity);

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to
            // visualize them.
            Session.GetTrackables<DetectedPlane>(_newPlanes, TrackableQueryFilter.New);
            for (int i = 0; i < _newPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The
                // transform is set to the origin with an identity rotation since the mesh for our
                // prefab is updated in Unity World coordinates.
                GameObject planeObject = Instantiate(
                    DetectedPlanePrefab, worldPose.position, worldPose.rotation, transform);
                planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(_newPlanes[i],null, m_Manager.planeColor, m_Manager.planeTextureARCore);

                if (!_isOriginPlaced)
                {
                    _planesBeforeOrigin.Add(planeObject);
                }
            }
        }

        /// <summary>
        /// Sets the apparent world origin of ARCore through applying an offset to the ARCoreDevice
        /// (and therefore also it's FirstPersonCamera child), so that the Origin of Unity's World
        /// Coordinate System coincides with the Anchor. This function needs to be called once the
        /// Cloud Anchor is either hosted or resolved.
        /// </summary>
        /// <param name="anchorTransform">Transform of the Cloud Anchor.</param>
        public void SetWorldOrigin(Transform anchorTransform)
        {
            // Each client will store the anchorTransform, and will have to move the ARCoreDevice
            // (and therefore also it's FirstPersonCamera child) and update other trakced poses
            // (planes, anchors, etc.) so that they appear in the same position in the real world.
            if (_isOriginPlaced)
            {
                Debug.LogWarning("The World Origin can be set only once.");
                return;
            }

            _isOriginPlaced = true;

            _anchorTransform = anchorTransform;

            Pose worldPose = WorldToAnchorPose(new Pose(ARCoreDeviceTransform.position,
                                                         ARCoreDeviceTransform.rotation));
            ARCoreDeviceTransform.SetPositionAndRotation(worldPose.position, worldPose.rotation);

            foreach (GameObject plane in _planesBeforeOrigin)
            {
                if (plane != null)
                {
                    plane.transform.SetPositionAndRotation(worldPose.position, worldPose.rotation);
                }
            }
        }

        /// <summary>
        /// Performs a raycast against physical objects being tracked by ARCore. This function wraps
        /// <c>Frame.Raycast</c> to add the necessary offset if the WorldOrigin is moved when a
        /// Cloud Anchor is placed.
        /// Output the closest hit from the camera.
        /// Note that the Unity's screen coordinate (0, 0) starts from bottom left.
        /// </summary>
        /// <param name="x">Horizontal touch position in Unity's screen coordinate.</param>
        /// <param name="y">Vertical touch position in Unity's screen coordinate.</param>
        /// <param name="filter">A filter bitmask where each set bit in
        /// <see cref="TrackableHitFlags"/>
        /// represents a category of raycast hits the method call should consider valid.</param>
        /// <param name="hitResult">A <see cref="TrackableHit"/> that will be set if the raycast is
        /// successful.</param>
        /// <returns><c>true</c> if the raycast had a hit, otherwise <c>false</c>.</returns>
        public bool Raycast(float x, float y, TrackableHitFlags filter, out TrackableHit hitResult)
        {
            bool foundHit = Frame.Raycast(x, y, filter, out hitResult);
            if (foundHit)
            {
                Pose worldPose = WorldToAnchorPose(hitResult.Pose);
                TrackableHit newHit = new TrackableHit(
                    worldPose, hitResult.Distance, hitResult.Flags, hitResult.Trackable);
                hitResult = newHit;
            }

            return foundHit;
        }

        /// <summary>
        /// Converts a pose from Unity world space to Anchor-relative space.
        /// </summary>
        /// <returns>A pose in Unity world space.</returns>
        /// <param name="pose">A pose in Anchor-relative space.</param>
        private Pose WorldToAnchorPose(Pose pose)
        {
            if (!_isOriginPlaced)
            {
                return pose;
            }

            Matrix4x4 anchorTWorld = Matrix4x4.TRS(
                _anchorTransform.position, _anchorTransform.rotation, Vector3.one).inverse;

            Vector3 position = anchorTWorld.MultiplyPoint(pose.position);
            Quaternion rotation = pose.rotation * Quaternion.LookRotation(
                anchorTWorld.GetColumn(2), anchorTWorld.GetColumn(1));

            return new Pose(position, rotation);
        }
    }
}
