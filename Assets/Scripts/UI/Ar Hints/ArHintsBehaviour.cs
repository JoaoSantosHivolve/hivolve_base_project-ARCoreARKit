using GoogleARCore;
using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ArHintsBehaviour : MonoBehaviour
{
    public bool ObjectsArePlaced
    {
        get
        {
            if(m_ObjectPlacer.PlacedObjectsCount > 0)
            {
                AlreadyPlacedAnObject = true;
                return true;
            }

            return false;
        }
    }
    public bool PlanesAreDetected
    {
        get
        {
            // Android
            if (ArManager.Instance.platform == Platform.Android)
            {
                Session.GetTrackables<DetectedPlane>(_detectedPlanes, TrackableQueryFilter.All);
                foreach (DetectedPlane plane in _detectedPlanes)
                {
                    if (plane.TrackingState == TrackingState.Tracking)
                    {
                        return true;
                    }
                }
                return false;
            }
            // iOS
            else
            {
                foreach (var plane in m_ARPlaneManager.trackables)
                {
                    if (plane != null)
                    {
                        return true;
                    }
                }
            }
            return false;
            }
        }
    public bool HasObjectToPlace
    {
        get
        {
            return m_ObjectPlacer.objectToInstantiate != null;
        }
    }
    public bool AlreadyPlacedAnObject
    {
        get { return m_AlreadyPlacedAnObject;}
        set
        {
            m_AlreadyPlacedAnObject = value;
        }
    }
    
    private bool m_AlreadyPlacedAnObject;
    private List<DetectedPlane> _detectedPlanes = new List<DetectedPlane>();
    private ObjectPlacer m_ObjectPlacer;
    private ARPlaneManager m_ARPlaneManager;

    private GameObject m_ScanVideo;
    private GameObject m_PlaceVideo;

    private void Awake()
    {
        m_ObjectPlacer = GameObject.FindObjectOfType<ObjectPlacer>();
        m_ARPlaneManager = GameObject.FindObjectOfType<ARPlaneManager>();

        m_ScanVideo = transform.GetChild(0).gameObject;
        m_PlaceVideo = transform.GetChild(1).gameObject;
    }

    private void Update()
    {
        // An Object was already placed, so the player knows how to work with the app
        if (m_AlreadyPlacedAnObject)
        {
            m_ScanVideo.SetActive(false);
            m_PlaceVideo.SetActive(false);
            return;
        }

        // If no object is selected no need to run tutorial
        if (!HasObjectToPlace)
            return;

        m_ScanVideo.SetActive(!PlanesAreDetected && !ObjectsArePlaced);
        m_PlaceVideo.SetActive(PlanesAreDetected && !ObjectsArePlaced);
    }
}
