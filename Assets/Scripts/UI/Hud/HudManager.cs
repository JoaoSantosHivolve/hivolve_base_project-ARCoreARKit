using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HudSide
{
    Left,
    Middle,
    Right
}

[ExecuteInEditMode]
public class HudManager : SingletonDestroyable<HudManager>
{
    public bool ObjectIsSelected
    {
        get
        {
            return ManipulationSystem.Instance.SelectedObject != null;
        }
    }

    private Animator m_AnimatorNS;
    private Animator m_AnimatorSelected;
    private GameObject m_HudButtonPrefab;
    public AppManager manager;
    [Header("--- NOT SELECTED --- Top")]
    public List<GameObject> nS_Top_LeftButtons;
    public List<GameObject> nS_Top_MiddleButtons;
    public List<GameObject> nS_Top_RightButtons;
    private Transform m_NS_TopParent;
    [Header("--- NOT SELECTED --- Bot")]
    public List<GameObject> nS_Bot_LeftButtons;
    public List<GameObject> nS_Bot_MiddleButtons;
    public List<GameObject> nS_Bot_RightButtons;
    private Transform m_NS_BotParent;

    [Header("--- SELECTED --- Top")]
    public List<GameObject> selectedTopLeftButtons;
    public List<GameObject> selectedTopMiddleButtons;
    public List<GameObject> selectedTopRightButtons;
    private Transform m_SelectedTopParent;

    [Header("--- SELECTED --- Bot")]
    public List<GameObject> selectedBottomLeftButtons;
    public List<GameObject> selectedBottomMiddleButtons;
    public List<GameObject> selectedBottomRightButtons;
    private Transform m_SelectedBottomParent;

    private void Awake()
    {
        m_AnimatorNS = transform.GetChild(0).GetComponent<Animator>();
        m_AnimatorSelected = transform.GetChild(1).GetComponent<Animator>();
        m_HudButtonPrefab = Resources.Load<GameObject>("Prefabs/Hud/HudButton");

        m_NS_TopParent = m_AnimatorNS.transform.GetChild(0).GetChild(1);
        m_NS_BotParent = m_AnimatorNS.transform.GetChild(1).GetChild(1);

        m_SelectedTopParent = m_AnimatorSelected.transform.GetChild(0).GetChild(1);
        m_SelectedBottomParent = m_AnimatorSelected.transform.GetChild(1).GetChild(1);
}
    private void Start()
    {
        SetupHud();
    }
    private void Update()
    {
        m_AnimatorNS.SetBool("Visible", !ObjectIsSelected);
        m_AnimatorSelected.SetBool("Visible", ObjectIsSelected);
    }

    public void SetupButtonsOnHud(HudSide side, ref List<GameObject> panelButtons, List<HudButtonType> buttonsToAdd, Transform parent)
    {
        // ----- Set "object selected" panel buttons
        // --- Left side
        // Start with a clean state
        DeleteButtonsOnList(ref panelButtons);
        // Cant add more than 3 buttons
        if (buttonsToAdd.Count > 3 || buttonsToAdd.Count == 0)
        {
            return;
        }
        // Instantiate buttons
        foreach (var item in buttonsToAdd)
        {
            // Instantiate button
            var newButton = Instantiate(m_HudButtonPrefab, parent);
            // Set name according to type of button
            newButton.name = "HudButton - " + item.ToString();
            // Add type of button script
            AddButtonScript(newButton, item);
            // Set correct color
            newButton.GetComponent<HudButton>().SetColor(manager.hudColor);
            // Set button sprites
            AddButtonSprites(newButton.GetComponent<HudButton>(), item);
            // Add button to list
            panelButtons.Add(newButton);
        }
        // Position buttons
        var count = panelButtons.Count;
        var size = manager.buttonsSize;
        var remainingSpace = 1f - (size * count);
        var increment = remainingSpace / (2 + (count - 1));
        float xAnchor;
        switch (side)
        {
            case HudSide.Left:
                xAnchor = manager.leftSideAnchor;
                break;
            case HudSide.Middle:
                xAnchor = 0.5f;
                break;
            case HudSide.Right:
                xAnchor = manager.rightSideAnchor;
                break;
            default:
                xAnchor = 0.5f;
                break;
        }
        for (int i = 0; i < count; i++)
        {
            var maxY = 1f - ((increment + (increment * i)) + (i * size));
            var minY = maxY - size;
            panelButtons[i].GetComponent<RectTransform>().anchorMin = new Vector2(xAnchor, minY);
            panelButtons[i].GetComponent<RectTransform>().anchorMax = new Vector2(xAnchor, maxY);
        }
    }
    private void AddButtonScript(GameObject button, HudButtonType type)
    {
        switch (type)
        {
            case HudButtonType.Delete:
                button.AddComponent<HudButtonDelete>();
                button.GetComponent<HudButtonDelete>().type = HudButtonOnClickType.OneClick;
                break;
            case HudButtonType.AutoRotate:
                button.AddComponent<HudButtonAutoRotate>();
                button.GetComponent<HudButtonAutoRotate>().type = HudButtonOnClickType.Toggle;
                break;
            case HudButtonType.Zoom:
                button.AddComponent<HudButtonZoom>();
                button.GetComponent<HudButtonZoom>().type = HudButtonOnClickType.Toggle;
                break;
            case HudButtonType.Confirm:
                button.AddComponent<HudButtonConfirm>();
                button.GetComponent<HudButtonConfirm>().type = HudButtonOnClickType.OneClick;
                break;
            case HudButtonType.Light:
                button.AddComponent<HudButtonLight>();
                button.GetComponent<HudButtonLight>().type = HudButtonOnClickType.Toggle;
                break;
            case HudButtonType.Tutorial:
                button.AddComponent<HudButtonTutorial>();
                button.GetComponent<HudButtonTutorial>().type = HudButtonOnClickType.OneClick;
                break;
            case HudButtonType.Print:
                button.AddComponent<HudButtonPrint>();
                button.GetComponent<HudButtonPrint>().type = HudButtonOnClickType.OneClick;
                break;
            default:
                break;
        }
    }
    private void AddButtonSprites(HudButton button, HudButtonType type)
    {
        var selectedIconPack = manager.iconPack.ToString();
        var path = "Images/Icon Packs/" + selectedIconPack + "/";

        switch (type)
        {
            case HudButtonType.Delete:
                button.DefaultSprite = Resources.Load<Sprite>(path + "Delete");
                break;
            case HudButtonType.AutoRotate:
                button.DefaultSprite = Resources.Load<Sprite>(path + "RotateDefault");
                button.ActivatedSprite = Resources.Load<Sprite>(path + "RotateActivated");
                break;
            case HudButtonType.Zoom:
                button.DefaultSprite = Resources.Load<Sprite>(path + "ZoomDefault");
                button.ActivatedSprite = Resources.Load<Sprite>(path + "ZoomActivated");
                break;
            case HudButtonType.Confirm:
                button.DefaultSprite = Resources.Load<Sprite>(path + "Confirm");
                break;
            case HudButtonType.Light:
                button.DefaultSprite = Resources.Load<Sprite>(path + "LightDefault");
                button.ActivatedSprite = Resources.Load<Sprite>(path + "LightActivated");
                break;
            case HudButtonType.Tutorial:
                button.DefaultSprite = Resources.Load<Sprite>(path + "TutorialDefault");
                break;
            case HudButtonType.Print:
                button.DefaultSprite = Resources.Load<Sprite>(path + "PrintDefault");
                break;
            default:
                break;
        }

    }
    private void DeleteButtonsOnList(ref List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            #if UNITY_EDITOR
            DestroyImmediate(list[i].gameObject);
#else
            if(list[i] != null)
            {
                Destroy(list[i].gameObject);
            }
#endif
        }

        list = new List<GameObject>();
    }

    public void SetupHud()
    {
        // Top Not Selected Panel
        SetupButtonsOnHud(HudSide.Left, ref nS_Top_LeftButtons, manager.ns_panel_Top_LeftButtons, m_NS_TopParent);
        SetupButtonsOnHud(HudSide.Middle, ref nS_Top_MiddleButtons, manager.ns_panel_Top_MiddleButtons, m_NS_TopParent);
        SetupButtonsOnHud(HudSide.Right, ref nS_Top_RightButtons, manager.ns_panel_Top_RightButtons, m_NS_TopParent);

        // Bottom Not Selected Panel
        SetupButtonsOnHud(HudSide.Left, ref nS_Bot_LeftButtons, manager.ns_panel_Bot_LeftButtons, m_NS_BotParent);
        SetupButtonsOnHud(HudSide.Middle, ref nS_Bot_MiddleButtons, manager.ns_panel_Bot_MiddleButtons, m_NS_BotParent);
        SetupButtonsOnHud(HudSide.Right, ref nS_Bot_RightButtons, manager.ns_panel_Bot_RightButtons, m_NS_BotParent);


        // Top Selected Panel
        SetupButtonsOnHud(HudSide.Left, ref selectedTopLeftButtons, manager.selectedPanelTopLeftButtons, m_SelectedTopParent);
        SetupButtonsOnHud(HudSide.Middle, ref selectedTopMiddleButtons, manager.selectedPanelTopMiddleButtons, m_SelectedTopParent);
        SetupButtonsOnHud(HudSide.Right, ref selectedTopRightButtons, manager.selectedPanelTopRightButtons, m_SelectedTopParent);

        // Bottom Selected Panel
        SetupButtonsOnHud(HudSide.Left, ref selectedBottomLeftButtons, manager.selectedPanelBotLeftButtons, m_SelectedBottomParent);
        SetupButtonsOnHud(HudSide.Middle, ref selectedBottomMiddleButtons, manager.selectedPanelBotMiddleButtons, m_SelectedBottomParent);
        SetupButtonsOnHud(HudSide.Right, ref selectedBottomRightButtons, manager.selectedPanelBotRightButtons, m_SelectedBottomParent);
    }
    public void SetButtonsVisibility(bool value)
    {
        var buttonsList = new List<HudButton>();

        foreach (var item in nS_Top_LeftButtons)
            buttonsList.Add(item.GetComponent<HudButton>());
        foreach (var item in nS_Top_MiddleButtons)
            buttonsList.Add(item.GetComponent<HudButton>());
        foreach (var item in nS_Top_RightButtons)
            buttonsList.Add(item.GetComponent<HudButton>());

        foreach (var item in nS_Bot_LeftButtons)
            buttonsList.Add(item.GetComponent<HudButton>());
        foreach (var item in nS_Bot_MiddleButtons)
            buttonsList.Add(item.GetComponent<HudButton>());
        foreach (var item in nS_Bot_RightButtons)
            buttonsList.Add(item.GetComponent<HudButton>());

        foreach (var item in selectedTopLeftButtons)
            buttonsList.Add(item.GetComponent<HudButton>());
        foreach (var item in selectedTopMiddleButtons)
            buttonsList.Add(item.GetComponent<HudButton>());
        foreach (var item in selectedTopRightButtons)
            buttonsList.Add(item.GetComponent<HudButton>());

        foreach (var item in selectedBottomLeftButtons)
            buttonsList.Add(item.GetComponent<HudButton>());
        foreach (var item in selectedBottomMiddleButtons)
            buttonsList.Add(item.GetComponent<HudButton>());
        foreach (var item in selectedBottomRightButtons)
            buttonsList.Add(item.GetComponent<HudButton>());

        foreach (var item in buttonsList)
        {
            item.SetButtonVisibility(value);
        }
    }
}
