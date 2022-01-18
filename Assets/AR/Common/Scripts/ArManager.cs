using GoogleARCore.Examples.Common;
using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;

public enum Platform
{
    Android,
    IOS,
}

public class ArManager : Singleton<ArManager>
{
    private Platform m_Platform;
    private GameObject m_ArCoreSection;
    private GameObject m_ArKitSection;

    
    [Header("----- Plane Generation Settings -----")]
    [Tooltip("Color of generated planes")]
    public Color planeColor;
    [Tooltip("Texture used on generated planes")]
    public Texture planeTexture;
    [Tooltip("Controls if planes are ever visible during the application")]
    public bool planesAlwaysHidden;
    [Tooltip("Controls if planes become invisble after placing an object")]
    public bool hidePlanesOnObjectPlacement;
    private DetectedPlaneGenerator m_ARCorePlaneGenerator;
    private PlaneDetectionController m_ARKitPlaneGenerator;

    [Header("----- Object Placer Settings -----")]
    private ObjectPlacer m_ARCoreObjectPlacer;
    private PlaceOnPlane m_ARKitObjectPlacer;

    protected override void Awake()
    {
        base.Awake();

        // Set platform
#if UNITY_ANDROID
        m_Platform = Platform.Android;
#elif UNITY_IOS
        m_Platform = Platform.IOS;
#endif
        // --- SECTION'S INITIALIZATION ---
        // Get sections GameObject's from children
        m_ArCoreSection = transform.GetChild(0).gameObject;
        m_ArKitSection = transform.GetChild(1).gameObject;

        // --- PLANE GENERATION INITIALIZATION ---
        // Get components
        m_ARCorePlaneGenerator = GameObject.FindObjectOfType<DetectedPlaneGenerator>();
        m_ARKitPlaneGenerator = GameObject.FindObjectOfType<PlaneDetectionController>();
        // Initialize components
        m_ARCorePlaneGenerator.planesAreVisible = !planesAlwaysHidden;
        m_ARKitPlaneGenerator.planesAreVisible = !planesAlwaysHidden;

        // --- OBJECT PLACEMENT INITIALIZATION ---
        // Get components
        m_ARCoreObjectPlacer = GameObject.FindObjectOfType<ObjectPlacer>();

        // --- ENABLE CORRECT SECTION
        // Set what section is enabled
        m_ArCoreSection.SetActive(m_Platform == Platform.Android);
        m_ArKitSection.SetActive(m_Platform == Platform.IOS);
    }


    public void SetPlanesVisibility(bool visible)
    {
        if (m_Platform == Platform.Android)
        {
            m_ARCorePlaneGenerator.SetVisibility(visible);
        }
        else if (m_Platform == Platform.IOS)
        {
            m_ARKitPlaneGenerator.SetVisibility(visible);
        }
    }
    public void SetObjectToInstantiate(GameObject objectToInstantiate)
    {
        if (m_Platform == Platform.Android)
        {
            m_ARCoreObjectPlacer.objectToInstantiate = objectToInstantiate;
        }
        else if (m_Platform == Platform.IOS)
        {

        }
    }

}
