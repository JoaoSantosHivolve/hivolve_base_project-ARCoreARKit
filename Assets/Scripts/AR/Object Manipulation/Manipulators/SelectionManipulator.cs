//-----------------------------------------------------------------------
// <copyright file="SelectionManipulator.cs" company="Google LLC">
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

namespace GoogleARCore.Examples.ObjectManipulation
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Controls the selection of an object through Tap gesture.
    /// </summary>
    public class SelectionManipulator : Manipulator
    {
        private PlacedObjectController m_ObjectManager;

        private float _scaledElevation;

        /// <summary>
        /// Should be called when the object elevation changes, to make sure that the Selection
        /// Visualization remains always at the plane level. This is the elevation that the object
        /// has, independently of the scale.
        /// </summary>
        /// <param name="elevation">The current object's elevation.</param>
        public void OnElevationChanged(float elevation)
        {
            _scaledElevation = elevation * transform.localScale.y;
            //SelectionVisualization.transform.localPosition = new Vector3(0, -elevation, 0);
        }

        /// <summary>
        /// Should be called when the object elevation changes, to make sure that the Selection
        /// Visualization remains always at the plane level. This is the elevation that the object
        /// has multiplied by the local scale in the y coordinate.
        /// </summary>
        /// <param name="scaledElevation">The current object's elevation scaled with the local y
        /// scale.</param>
        public void OnElevationChangedScaled(float scaledElevation)
        {
            _scaledElevation = scaledElevation;
            //SelectionVisualization.transform.localPosition =
                //new Vector3(0, -scaledElevation / transform.localScale.y, 0);
        }

        private void Start()
        {
            m_ObjectManager = transform.GetChild(0).GetComponent<PlacedObjectController>();
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        protected override void Update()
        {
            base.Update();
            if (transform.hasChanged)
            {
                float height = -_scaledElevation / transform.localScale.y;
                //SelectionVisualization.transform.localPosition = new Vector3(0, height, 0);
            }
        }

        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>True if the manipulation can be started.</returns>
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            return true;
        }

        /// <summary>
        /// Function called when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(TapGesture gesture)
        {
            if (gesture.WasCancelled)
            {
                return;
            }

            if (ManipulationSystem.Instance == null)
            {
                return;
            }

            GameObject target = gesture.TargetObject;
            if (target == gameObject)
            {
                Select();
            }

            // Prevent deselect when pressing UI buttons
            if (IsPointerOverUIObject())
                return;
            if (IsPointerOverUiObject(gesture))
                return;

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

            if (!Frame.Raycast(gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit))
            {
                Deselect();
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

        /// <summary>
        /// Function called when this game object is deselected if it was the Selected Object.
        /// </summary>
        protected override void OnSelected()
        {
            m_ObjectManager.SetObjectAnimation(true);
            m_ObjectManager.SetSelectionVisualizer(true);
        }

        /// <summary>
        /// Function called when this game object is deselected if it was the Selected Object.
        /// </summary>
        protected override void OnDeselected()
        {
            m_ObjectManager.SetObjectAnimation(false);
            m_ObjectManager.SetSelectionVisualizer(false);
        }
    }
}
