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
public enum IconPack
{
    Default
}
public enum HudButtonType
{
    Delete,
    AutoRotate,
    Zoom,
    Confirm
}

public class ArManager : Singleton<ArManager>
{
    public Platform platform { get; private set; }
    private GameObject m_ArCoreSection;
    private GameObject m_ArKitSection;
    
    [Header("----- Plane Generation Settings -----")]
    public Color planeColor;
    public Texture planeTextureARCore;
    public Texture planeTextureARKit;
    public PlanesVisibility planesVisibility;
    private DetectedPlaneGenerator m_ARCorePlaneGenerator;
    private PlaneDetectionController m_ARKitPlaneGenerator;

    [Header("----- Point Cloud Settings -----")]
    public Color pointCloudColor;
    private PointcloudVisualizer m_ARCorePointCloudVisualizer;
    private ARPointCloudManager m_ARKitPointCloudVisualizer;

    [Header("----- Object Placer Settings -----")]
    public Sprite selectionVisualizerSprite;
    public Color selectionVisualizerColor;
    public bool objectFaceCameraOnPlacement;
    public bool canRotateIfAutoRotateIsEnabled;
    [Range(0.1f, 1f)] public float autoRotationSpeed;
    public bool canScaleIfZoomIsEnabled;
    [Range(2f, 10f)] public float zoomAmount;
    private ObjectPlacer m_ObjectPlacer;
    [HideInInspector] public ARRaycastManager arKitRaycaster;

    [Header("----- Hud Settings -----")]
    public Color hudColor;
    public IconPack iconPack;
    [Range(0.2f, 0.3f)] public float buttonsSize;
    [Range(0.1f, 0.4f)] public float leftSideAnchor;
    [Range(0.6f, 0.9f)] public float rightSideAnchor;
    public List<HudButtonType> selectedPanelLeftButtons;
    public List<HudButtonType> selectedPanelMiddleButtons;
    public List<HudButtonType> selectedPanelRightButtons;

    protected override void Awake()
    {
        base.Awake();

        // Set platform
#if UNITY_ANDROID
        platform = Platform.Android;
#elif UNITY_IOS
        platform = Platform.IOS;
#endif
        // ----- SECTION'S INITIALIZATION -----
        m_ArCoreSection = transform.GetChild(0).gameObject;
        m_ArKitSection = transform.GetChild(1).gameObject;

        // ----- PLANE GENERATION INITIALIZATION -----
        m_ARCorePlaneGenerator = GameObject.FindObjectOfType<DetectedPlaneGenerator>();
        m_ARKitPlaneGenerator = GameObject.FindObjectOfType<PlaneDetectionController>();
        m_ARCorePlaneGenerator.planesAreVisible = planesVisibility != PlanesVisibility.AlwaysHide;
        m_ARKitPlaneGenerator.planesAreVisible = planesVisibility != PlanesVisibility.AlwaysHide;

        // ----- POINT CLOUD INITIALIZATION -----
        m_ARCorePointCloudVisualizer = GameObject.FindObjectOfType<PointcloudVisualizer>();
        m_ARKitPointCloudVisualizer = GameObject.FindObjectOfType<ARPointCloudManager>();
        m_ARCorePointCloudVisualizer.PointColor = pointCloudColor;
        //TODO: ARKit

        // ----- OBJECT PLACEMENT INITIALIZATION -----
        m_ObjectPlacer = GameObject.FindObjectOfType<ObjectPlacer>();
        arKitRaycaster = GameObject.FindObjectOfType<ARRaycastManager>();

        // ----- ENABLE CORRECT SECTION -----
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
    public void SetObjectToInstantiate(PlacedObjectManager objectToInstantiate) => m_ObjectPlacer.objectToInstantiate = objectToInstantiate;
    public void DeselectObject() => ManipulationSystem.Instance.Deselect();
    public void DeleteAllObjects() => m_ObjectPlacer.DeleteAllObjects();
    public void DeleteSelectedObject() => m_ObjectPlacer.DeleteSelectedObject();

    [ContextMenu("UPDATE HUD")]
    void SetupHudViaInspector()
    {
        GameObject.FindObjectOfType<HudManager>().SetupHud();
    }
}