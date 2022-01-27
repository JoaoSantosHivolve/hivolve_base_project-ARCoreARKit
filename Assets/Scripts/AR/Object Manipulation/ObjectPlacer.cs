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
    using UnityEngine.EventSystems;
    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;

    public class ObjectPlacer : Manipulator
    {
        private Camera m_FirstPersonCamera;
        public PlacedObjectManager objectToInstantiate;
        public GameObject ManipulatorPrefab;

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        private ARRaycastManager m_RaycastManager;

        [SerializeField] private List<GameObject> m_PlacedObjects = new List<GameObject>();

        private void Awake()
        {
            // Get main camera
            m_FirstPersonCamera = Camera.main;
            m_RaycastManager = GameObject.FindObjectOfType<ARRaycastManager>();
        }

        protected override void Update()
        {
            base.Update();

            // Control planes Visibility
            switch (ArManager.Instance.planesVisibility)
            {
                case PlanesVisibility.AlwaysVisible:
                    ArManager.Instance.SetPlanesVisibility(true);
                    break;
                case PlanesVisibility.HideOnObjectPlacement:
                    ArManager.Instance.SetPlanesVisibility(m_PlacedObjects.Count == 0);
                    break;
                case PlanesVisibility.ShowWhenInteracting:
                    if(m_PlacedObjects.Count == 0)
                    {
                        ArManager.Instance.SetPlanesVisibility(true);
                    }
                    else
                    {
                        ArManager.Instance.SetPlanesVisibility(ManipulationSystem.Instance.SelectedObject != null);
                    }
                    break;
                case PlanesVisibility.AlwaysHide:
                    ArManager.Instance.SetPlanesVisibility(false);
                    break;
                default:
                    break;
            }
        }

        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            if (gesture.TargetObject == null)
            {
                return true;
            }
            return false;
        }
        protected override void OnEndManipulation(TapGesture gesture)
        {
            if (objectToInstantiate == null)
                return;

            if (gesture.WasCancelled)
            {
                return;
            }

            // ----- Object Detection -----
            // If gesture is targeting an existing object we are done.
            if (gesture.TargetObject != null)
                return;
            // Double Check object targetting
            if (IsPointerOverGameObject(gesture.StartPosition))
                return;
            // Check if is inside Object
            if (IsPointerInsideObject(gesture.StartPosition))
                return;

            // ----- UI Detection -----
            if (IsPointerOverUIObject())
                return;
            if (IsPointerOverUiObject(gesture))
                return;

            // ----- Raycast Android -----
            if (ArManager.Instance.platform == Platform.Android)
            {
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
            // ----- Raycast iOS -----
            else if (ArManager.Instance.platform == Platform.IOS)
            {
                if (m_RaycastManager.Raycast(gesture.StartPosition, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    var hitPose = s_Hits[0].pose;

                    PlaceObject(hitPose.position, hitPose.rotation, new TrackableHit());
                }

                //if (Physics.Raycast(Camera.main.ScreenToWorldPoint(gesture.StartPosition), Camera.main.transform.forward, out var hit))
                //{
                //    if (hit.collider.gameObject != null)
                //    {
                //        if (hit.collider.gameObject.GetComponent<ARPlane>() != null)
                //        {
                //            PlaceObject(hit.point, Quaternion.identity, new TrackableHit());
                //        }
                //    }
                //}
            }
        }

        private static bool IsPointerOverUiObject(TapGesture gesture)
        {
            // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
            // the ray cast appears to require only eventData.position.
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(gesture.StartPosition.x, gesture.StartPosition.y)
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        private bool IsPointerOverGameObject(Vector2 position)
        {
            var ray = Camera.main.ScreenPointToRay(position);

            return Physics.Raycast(ray, out var hit) ? hit.transform.parent.GetComponent<Manipulator>() : false;
        }
        private bool IsPointerInsideObject(Vector2 position)
        {
            var ray = Camera.main.ScreenPointToRay(position);
            ray.origin = ray.GetPoint(100);
            ray.direction = -ray.direction;

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.parent.GetComponent<Manipulator>())
                {
                    hit.transform.parent.GetComponent<Manipulator>().Select();
                    return true;
                }
                else return false;
            }
            else return false;
        }

        private void PlaceObject(Vector3 position, Quaternion rotation, TrackableHit hit)
        {
            // Delete old objects
            DeleteAllObjects();

            // Instantiate game object at the hit pose.
            var placedObject = Instantiate(objectToInstantiate, position, rotation);

            // Instantiate manipulator.
            var manipulator = Instantiate(ManipulatorPrefab, position, rotation);

            // Make game object a child of the manipulator.
            placedObject.transform.parent = manipulator.transform;

            // Create anchor based on selected platform
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

            // Add new placed object to list - Manipulator parent ( anchor )
            m_PlacedObjects.Add(manipulator.transform.parent.gameObject);

            // Set selection visualizer sprite and color
            placedObject.SetSelectionVisualizerColor(ArManager.Instance.selectionVisualizerColor);
            placedObject.SetSelectionVisualizerSprite(ArManager.Instance.selectionVisualizerSprite);

            // Object face camera
            if (ArManager.Instance.objectFaceCameraOnPlacement)
            {
                placedObject.transform.LookAt(Camera.main.transform.position);
                var lea = placedObject.transform.localEulerAngles;
                lea.z = 0; lea.x = 0;
                placedObject.transform.localEulerAngles = lea;
            }
        }

        public void DeleteAllObjects()
        {
            for (int i = 0; i < m_PlacedObjects.Count; i++)
            {
                Destroy(m_PlacedObjects[i]);
            }

            m_PlacedObjects = new List<GameObject>();
        }
        public void DeleteSelectedObject()
        {
            // Remove object from placed objects list
            m_PlacedObjects.Remove(ManipulationSystem.Instance.SelectedObject);

            // Destroy object
            Destroy(ManipulationSystem.Instance.SelectedObject);

            // Set selected object to null
            ManipulationSystem.Instance.Deselect();
        }
    }

    /*
    Android:
        1.Anchor
            2.Manipulator
                3.Object
    iOS:
        1.Empty GameObject (makes arcore manipulators easier to work with, and object deletion easier too) 
            2.Manipulator
                3.Object
     */
}
