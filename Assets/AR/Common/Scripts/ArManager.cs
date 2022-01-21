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
    IOS
}
public enum PlanesVisibility
{
    AlwaysVisible,
    HideOnObjectPlacement,
    AlwaysHide
}

public class ArManager : Singleton<ArManager>
{
    public Platform platform { get; private set; }
    private GameObject m_ArCoreSection;
    private GameObject m_ArKitSection;

    
    [Header("----- Plane Generation Settings -----")]
    [Tooltip("Color of generated planes")]
    public Color planeColor;
    [Tooltip("Texture used on generated planes")]
    public Texture planeTexture;
    [Tooltip("Controls the planes visibility during the experince")]
    public PlanesVisibility planesVisibility;
    private DetectedPlaneGenerator m_ARCorePlaneGenerator;
    private PlaneDetectionController m_ARKitPlaneGenerator;

    [Header("----- Object Placer Settings -----")]
    private ObjectPlacer m_ObjectPlacer;
    [HideInInspector] public ARRaycastManager arKitRaycaster;

    protected override void Awake()
    {
        base.Awake();

        // Set platform
#if UNITY_ANDROID
        platform = Platform.Android;
#elif UNITY_IOS
        platform = Platform.IOS;
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
        m_ARCorePlaneGenerator.planesAreVisible = planesVisibility != PlanesVisibility.AlwaysHide;
        m_ARKitPlaneGenerator.planesAreVisible = planesVisibility != PlanesVisibility.AlwaysHide;

        // --- OBJECT PLACEMENT INITIALIZATION ---
        // Get components
        m_ObjectPlacer = GameObject.FindObjectOfType<ObjectPlacer>();
        arKitRaycaster = GameObject.FindObjectOfType<ARRaycastManager>();

        // --- ENABLE CORRECT SECTION
        // Set what section is enabled
        m_ArCoreSection.SetActive(platform == Platform.Android);
        m_ArKitSection.SetActive(platform == Platform.IOS);
    }


    public void SetPlanesVisibility(bool visible)
    {
        if (platform == Platform.Android)
        {
            m_ARCorePlaneGenerator.SetVisibility(visible);
        }
        else if (platform == Platform.IOS)
        {
            m_ARKitPlaneGenerator.SetVisibility(visible);
        }
    }
    public void SetObjectToInstantiate(GameObject objectToInstantiate) => m_ObjectPlacer.objectToInstantiate = objectToInstantiate;
    public void DeselectObject() => ManipulationSystem.Instance.Deselect();
    public void DeleteAllObjects() => m_ObjectPlacer.DeleteAllObjects();
    public void DeleteSelectedObject() => m_ObjectPlacer.DeleteSelectedObject();

}
