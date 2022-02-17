using GoogleARCore.Examples.ObjectManipulation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class PlaneDetectionController : MonoBehaviour
    {
        private ARPlaneManager m_ARPlaneManager;
        private ObjectPlacer m_ObjPlacer;
        public bool planesAreVisible;
        public Material planeMaterial;

        private void Awake()
        {
            m_ARPlaneManager = GameObject.FindObjectOfType<ARPlaneManager>();
            m_ObjPlacer = GameObject.FindObjectOfType<ObjectPlacer>();
        }

        private void LateUpdate()
        {
            foreach (var plane in m_ARPlaneManager.trackables)
            {
                if(plane != null)
                {
                    // Set visibility
                    plane.gameObject.GetComponent<MeshRenderer>().enabled = planesAreVisible;
                    //plane.gameObject.GetComponent<LineRenderer>().enabled = planesAreVisible;

                    // Set color
                    var color = AppManager.Instance.planeColor;
                    plane.gameObject.GetComponent<MeshRenderer>().material = planeMaterial;
                    plane.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
                    //plane.gameObject.GetComponent<LineRenderer>().startColor = color;
                    //plane.gameObject.GetComponent<LineRenderer>().endColor = color;

                    // Set texture
                    var texture = AppManager.Instance.planeTextureARKit;
                    plane.gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;

                    if (m_ObjPlacer.placedObject != null && AppManager.Instance.addObjectInPlaneEffect)
                    {
                        plane.gameObject.GetComponent<MeshRenderer>().material.SetVector("_Position", m_ObjPlacer.placedObject.transform.position);
                    }
                }
            }
        }

        public void TogglePlaneDetection()
        {
            m_ARPlaneManager.enabled = !m_ARPlaneManager.enabled;

            if (m_ARPlaneManager.enabled)
            {
                SetAllPlanesActive(true);
            }
            else
            {
                SetAllPlanesActive(false);
            }
        }

        public void SetVisibility(bool visible)
        {
            planesAreVisible = visible;
        }
        public void SetEffect(bool state)
        {
            planeMaterial = state ? AppManager.Instance.planeMaterialARKitEffect : AppManager.Instance.planeMaterialARKitDefault;
        }
        /// <summary>
        /// Iterates over all the existing planes and activates
        /// or deactivates their <c>GameObject</c>s'.
        /// </summary>
        /// <param name="value">Each planes' GameObject is SetActive with this value.</param>
        void SetAllPlanesActive(bool value)
        {
            foreach (var plane in m_ARPlaneManager.trackables)
                plane.gameObject.SetActive(value);
        }
    }
}