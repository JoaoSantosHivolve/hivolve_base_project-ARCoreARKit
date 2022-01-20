//-----------------------------------------------------------------------
// <copyright file="PawnManipulator.cs" company="Google LLC">
//
// Copyright 2019 Google LLC
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

namespace GoogleARCore.Examples.ObjectManipulation
{
    using GoogleARCore;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;

    public class ObjectPlacer : Manipulator
    {
        private Camera m_FirstPersonCamera;

        public GameObject objectToInstantiate;

        /// <summary>
        /// Manipulator prefab to attach placed objects to.
        /// </summary>
        public GameObject ManipulatorPrefab;

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        ARRaycastManager m_RaycastManager;

        private void Awake()
        {
            m_FirstPersonCamera = Camera.main;
        }

        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            Debug.Log("ObjectPlacer: can start");
            if (gesture.TargetObject == null)
            {
                Debug.Log("ObjectPlacer: can start true");
                return true;
            }
            Debug.Log("ObjectPlacer: can start false");
            return false;
        }

        /// <summary>
        /// Function called when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(TapGesture gesture)
        {
            Debug.Log("ObjectPlacer: 1");
            if (gesture.WasCancelled)
            {
                return;
            }
            Debug.Log("ObjectPlacer: 2");
            // If gesture is targeting an existing object we are done.
            if (gesture.TargetObject != null)
            {
                return;
            }
            Debug.Log("ObjectPlacer: 3");
            // --- Raycast Android
            if (ArManager.Instance.platform == Platform.Android)
            {
                Debug.Log("ObjectPlacer: 4");
                if (Frame.Raycast(gesture.StartPosition.x, gesture.StartPosition.y, TrackableHitFlags.PlaneWithinPolygon, out var hit))
                {
                    // Use hit pose and camera pose to check if hittest is from the
                    // back of the plane, if it is, no need to create the anchor.
                    if ((hit.Trackable is DetectedPlane) &&
                        Vector3.Dot(m_FirstPersonCamera.transform.position - hit.Pose.position,
                            hit.Pose.rotation * Vector3.up) < 0)
                    {
                        Debug.Log("Hit at back of the current DetectedPlane");
                    }
                    else
                    {
                        PlaceObject(hit.Pose.position, hit.Pose.rotation, hit);
                    }
                }
            }
            // --- Raycast iOS
            else if (ArManager.Instance.platform == Platform.IOS)
            {
                    Debug.Log("ObjectPlacer: 5");
                if(Physics.Raycast(Camera.main.ScreenToWorldPoint(gesture.StartPosition), Camera.main.transform.forward, out var hit))
                {
                    Debug.Log("ObjectPlacer: 6");
                    if (hit.collider.gameObject != null)
                    {
                        Debug.Log("ObjectPlacer: 7");
                        if (hit.collider.gameObject.GetComponent<ARPlane>() != null)
                        {
                            Debug.Log("ObjectPlacer: 8");
                            PlaceObject(hit.point, Quaternion.identity, new TrackableHit());
                        }
                    }
                }
               //if (Frame.Raycast(gesture.StartPosition.x, gesture.StartPosition.y, TrackableHitFlags.PlaneWithinPolygon, out var hit))
               //{
               // //   if (m_RaycastManager.Raycast(gesture.StartPosition, s_Hits, TrackableType.PlaneWithinPolygon))
               // //{
               //    Debug.Log("ObjectPlacer: 6");
               //    //var hitPose = s_Hits[0].pose;
               //    //PlaceObject(hitPose.position, hitPose.rotation, new TrackableHit());
               //    PlaceObject(hit.Pose.position, hit.Pose.rotation, new TrackableHit());
               //}
            }
        }

        private void PlaceObject(Vector3 position, Quaternion rotation, TrackableHit hit)
        {
            Debug.Log("ObjectPlacer: Final");
            // Instantiate game object at the hit pose.
            var gameObject = Instantiate(objectToInstantiate, position, rotation);

            // Instantiate manipulator.
            var manipulator = Instantiate(ManipulatorPrefab, position, rotation);

            // Make game object a child of the manipulator.
            gameObject.transform.parent = manipulator.transform;

            // Create an anchor to allow ARCore to track the hitpoint as understanding of
            // the physical world evolves.
            if (ArManager.Instance.platform == Platform.Android)
            {
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                manipulator.transform.parent = anchor.transform;
            }
            else
            {
                var anchor = Instantiate(new GameObject(), position, rotation);
                manipulator.transform.parent = anchor.transform;
            }

            // Select the placed object.
            manipulator.GetComponent<Manipulator>().Select();

            // Hide planes if the setting is enabled
            if (ArManager.Instance.hidePlanesOnObjectPlacement)
            {
                ArManager.Instance.SetPlanesVisibility(false);
            }
        }
    }
    /*
    Android:
        1.Anchor
            2.Manipulator
                3.Object
    iOS:
        1.Manipulator
            2.Object
     */
}
