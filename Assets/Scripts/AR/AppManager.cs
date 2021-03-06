using GoogleARCore.Examples.Common;
using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    ShowWhenInteracting,
    AlwaysHide
}
public enum PointCloudVisibility
{
    AlwaysVisible,
    HideOnObjectPlacement,
    AlwaysHide
}
public enum IconPack
{
    Default
}
public enum LoadingUI
{
    Dots,
    Line,
    Circle
}
public enum HudButtonType
{
    Delete,
    AutoRotate,
    Zoom,
    Confirm,
    Light,
    Tutorial,
    Print
}

public class AppManager : Singleton<AppManager>
{
    public Platform platform { get; private set; }
    private GameObject m_ArCoreSection;
    private GameObject m_ArKitSection;

    [Header("----- Language Settings -----")]
    public Language appLanguage;

    [Header("----- Plane Generation Settings -----")]
    public PlanesVisibility planesVisibility;
    public Color planeColor;
    public Texture planeTextureARCore;
    public Texture planeTextureARKit;
    [Tooltip("Only works with Always Visible")] public bool addObjectInPlaneEffect;
    public Material planeMaterialARCoreDefault;
    public Material planeMaterialARCoreEffect;
    public Material planeMaterialARKitDefault;
    public Material planeMaterialARKitEffect;
    private DetectedPlaneGenerator m_ARCorePlaneGenerator;
    private PlaneDetectionController m_ARKitPlaneGenerator;
   
    [Header("----- Object Placer Settings -----")]
    public Color selectionVisualizerColor;
    public Sprite selectionVisualizerSprite;
    public bool objectFaceCameraOnPlacement;

    [Header("----- Manipulation Settings -----")]
    public bool canRotateIfAutoRotateIsEnabled;
    public bool canScaleIfZoomIsEnabled;
    [Range(0.1f, 1f)]   public float autoRotationSpeed;
    [Range(2f, 10f)]    public float zoomAmount;
    [Range(0.1f, 1.0f)] public float minScale;
    [Range(1.0f, 2.0f)] public float maxScale;
    private ObjectPlacer m_ObjectPlacer;
    [HideInInspector] public GameObject m_ExtraLights; // Public so it can be used on lightButton

    [Header("----- UI - Intro Settings -----")]
    public LoadingUI loadingType;
    public bool playIntroSound;
    [Range(1.00f, 5.00f)] public float loadingTime;

    [Header("----- UI - Print Settings -----")]
    public bool hideHudWhenTakingPrintscreen;
    public bool applyOverlay;
    public string overlayText;

    [Header("----- UI - Hud Settings -----")]
    public IconPack iconPack;
    public Color hudColor;
    [Range(0.0f, 1.0f)] public float buttonVolume;
    [Range(0.2f, 0.3f)] public float buttonsSize;
    [Range(0.1f, 0.4f)] public float leftSideAnchor;
    [Range(0.6f, 0.9f)] public float rightSideAnchor;
    public List<HudButtonType> ns_panel_Top_LeftButtons;
    public List<HudButtonType> ns_panel_Top_MiddleButtons;
    public List<HudButtonType> ns_panel_Top_RightButtons;
    public List<HudButtonType> ns_panel_Bot_LeftButtons;
    public List<HudButtonType> ns_panel_Bot_MiddleButtons;
    public List<HudButtonType> ns_panel_Bot_RightButtons;
    public List<HudButtonType> selectedPanelTopLeftButtons;
    public List<HudButtonType> selectedPanelTopMiddleButtons;
    public List<HudButtonType> selectedPanelTopRightButtons;
    public List<HudButtonType> selectedPanelBotLeftButtons;
    public List<HudButtonType> selectedPanelBotMiddleButtons;
    public List<HudButtonType> selectedPanelBotRightButtons;

    protected override void Awake()
    {
        base.Awake();
        // Set platform
#if UNITY_ANDROID
        platform = Platform.Android;
#elif UNITY_IOS
        platform = Platform.IOS;
#endif
        // ----- INITIALIZE ALL MANAGERS -----
        GameObject.FindObjectOfType<LanguageManager>().InitializeDestroyableSingleton();
        GameObject.FindObjectOfType<IntroManager>().InitializeDestroyableSingleton();
        GameObject.FindObjectOfType<PrintManager>().InitializeDestroyableSingleton();
        GameObject.FindObjectOfType<HudManager>().InitializeDestroyableSingleton();
        GameObject.FindObjectOfType<ArHintsManager>().InitializeDestroyableSingleton();

        LanguageManager.Instance.Init();
        IntroManager.Instance.Init();
        HudManager.Instance.Init();
        ArHintsManager.Instance.Init();
        PrintManager.Instance.Init();

        // ----- SECTION'S INITIALIZATION -----
        m_ArCoreSection = transform.GetChild(0).GetChild(0).gameObject;
        m_ArKitSection = transform.GetChild(0).GetChild(1).gameObject;

        // ----- PLANE GENERATION INITIALIZATION -----
        m_ARCorePlaneGenerator = GameObject.FindObjectOfType<DetectedPlaneGenerator>();
        m_ARKitPlaneGenerator = GameObject.FindObjectOfType<PlaneDetectionController>();
        m_ARCorePlaneGenerator.planeMaterial = planeMaterialARCoreDefault;
        m_ARKitPlaneGenerator.planeMaterial = planeMaterialARKitDefault;
        // ----- MANIPULATION INITIALIZATION -----
        m_ExtraLights = GameObject.Find("Lights");
        m_ExtraLights.SetActive(false);

        // ----- OBJECT PLACEMENT INITIALIZATION -----
        m_ObjectPlacer = GameObject.FindObjectOfType<ObjectPlacer>();

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
    public void SetPlanesEffect(bool state)
    {
        if (platform == Platform.Android)
        {
            m_ARCorePlaneGenerator.SetEffect(state);
        }
        else if (platform == Platform.IOS)
        {
            m_ARKitPlaneGenerator.SetEffect(state);
        }
    }
    public void SetObjectToInstantiate(PlacedObjectController objectToInstantiate) => m_ObjectPlacer.objectToInstantiate = objectToInstantiate;
    public void DeselectObject() => ManipulationSystem.Instance.Deselect();
    public void DeleteAllObjects() => m_ObjectPlacer.DeleteAllObjects();
    public void DeleteSelectedObject() => m_ObjectPlacer.DeleteSelectedObject();
    public void SetExtraLights(bool value) => m_ExtraLights.SetActive(value);

    [ContextMenu("UPDATE HUD")]
    void SetupHudViaInspector()
    {
        GameObject.FindObjectOfType<HudManager>().SetupHud();
    }

    [ContextMenu("UPDATE LANGUAGE")]
    void UpdateLanguage()
    {
#if UNITY_EDITOR
        GameObject.FindObjectOfType<LanguageManager>().Language = appLanguage;
#else
        LanguageManager.Instance.Language = appLanguage;
#endif

    }
}