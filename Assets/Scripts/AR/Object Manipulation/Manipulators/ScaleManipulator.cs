//-----------------------------------------------------------------------
// <copyright file="ScaleManipulator.cs" company="Google LLC">
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
    using GoogleARCore.Examples.ObjectManipulationInternal;
    using UnityEngine;

    /// <summary>
    /// Controls the scale of an object via a Pinch gesture.
    /// If an object is selected, then doing a pinch/zoom modify the scale
    /// of the object.
    /// </summary>
    public class ScaleManipulator : Manipulator
    {
        public float minScale => AppManager.Instance.minScale;
        public float maxScale => AppManager.Instance.maxScale;

        private const float _elasticRatioLimit = 0.8f;
        private const float _sensitivity = 0.75f;
        private const float _elasticity = 0.15f;

        private float _currentScaleRatio;

        private bool m_IsScaling;
        private bool IsScaling 
        {
            get { return m_IsScaling; }
            set 
            { 
                m_IsScaling = value;

                m_ObjectManager.SetScaleVisualizer(value);
            }
        }

        private PlacedObjectController m_ObjectManager;

        private float _scaleDelta
        {
            get
            {
                if (minScale > maxScale)
                {
                    Debug.LogError("minScale must be smaller than maxScale.");
                    return 0.0f;
                }

                return maxScale - minScale;
            }
        }
        private float _clampedScaleRatio
        {
            get
            {
                return Mathf.Clamp01(_currentScaleRatio);
            }
        }
        private float _currentScale
        {
            get
            {
                float elasticScaleRatio = _clampedScaleRatio + ElasticDelta();
                float elasticScale = minScale + (elasticScaleRatio * _scaleDelta);

                return elasticScale;
            }
        }

        private bool m_IsZoomed;
        public bool IsZoomed
        {
            get { return m_IsZoomed; }
            set
            {
                m_IsZoomed = value;

                // Zoom placed object
                transform.GetChild(0).localScale = value ? Vector3.one * AppManager.Instance.zoomAmount : Vector3.one;

                // Reset scale
                transform.localScale = Vector3.one;
                _currentScaleRatio = (transform.localScale.x - minScale) / _scaleDelta;
                // Show visualizer
                m_ObjectManager.SetScaleVisualizer(true);
                // Update value on scale visualizer
                UpdateVisualizerValue();
            }
        }

        private void Start()
        {
            m_ObjectManager = transform.GetChild(0).GetComponent<PlacedObjectController>();
        }

        /// <summary>
        /// Enabled the scale controller.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _currentScaleRatio = (transform.localScale.x - minScale) / _scaleDelta;
        }

        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>True if the manipulation can be started.</returns>
        protected override bool CanStartManipulationForGesture(PinchGesture gesture)
        {
            if (!IsSelected())
            {
                return false;
            }

            if (gesture.TargetObject != null)
            {
                return false;
            }

            if(IsZoomed && !AppManager.Instance.canScaleIfZoomIsEnabled)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Recalculates the current scale ratio in case local scale, min or max scale were changed.
        /// </summary>
        /// <param name="gesture">The gesture that started this transformation.</param>
        protected override void OnStartManipulation(PinchGesture gesture)
        {
            IsScaling = true;
            _currentScaleRatio = (transform.localScale.x - minScale) / _scaleDelta;
        }

        /// <summary>
        /// Continues the scaling of the object.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnContinueManipulation(PinchGesture gesture)
        {
            _currentScaleRatio += _sensitivity * GestureTouchesUtility.PixelsToInches(gesture.GapDelta);

            float currentScale = _currentScale;
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            // If we've tried to scale too far beyond the limit, then cancel the gesture
            // to snap back within the scale range.
            if (_currentScaleRatio < -_elasticRatioLimit || _currentScaleRatio > (1.0f + _elasticRatioLimit))
            {
                gesture.Cancel();
            }
        }

        /// <summary>
        /// Finishes the scaling of the object.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(PinchGesture gesture)
        {
            IsScaling = false;
        }

        private float ElasticDelta()
        {
            float overRatio = 0.0f;
            if (_currentScaleRatio > 1.0f)
            {
                overRatio = _currentScaleRatio - 1.0f;
            }
            else if (_currentScaleRatio < 0.0f)
            {
                overRatio = _currentScaleRatio;
            }
            else
            {
                return 0.0f;
            }

            return (1.0f - (1.0f / ((Mathf.Abs(overRatio) * _elasticity) + 1.0f)))
            * Mathf.Sign(overRatio);
        }

        private void LateUpdate()
        {
            if (!IsScaling)
            {
                _currentScaleRatio = Mathf.Lerp(_currentScaleRatio, _clampedScaleRatio, Time.deltaTime * 8.0f);
                float currentScale = _currentScale;

                transform.localScale = new Vector3(currentScale, currentScale, currentScale);

                m_ObjectManager.SetScaleVisualizer(false);
            }
            else
            {
                UpdateVisualizerValue();
            }
        }

        private void UpdateVisualizerValue()
        {
            m_ObjectManager.SetScaleVisualizerValue(IsZoomed ? _currentScale * AppManager.Instance.zoomAmount : _currentScale);
        }
    }
}