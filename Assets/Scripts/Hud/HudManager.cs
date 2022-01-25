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
public class HudManager : MonoBehaviour
{
    public bool ObjectIsSelected
    {
        get
        {
            return ManipulationSystem.Instance.SelectedObject != null;
        }
    }

    private Animator m_AnimatorNotSelected;
    private Animator m_AnimatorSelected;
    private GameObject m_HudButtonPrefab;
    public ArManager manager;
    [Header("Selected Panel Top")]
    private Transform m_SelectedTopParent;
    public List<GameObject> selectedTopLeftButtons;
    public List<GameObject> selectedTopMiddleButtons;
    public List<GameObject> selectedTopRightButtons;
    [Header("Selected Panel Bottom")]
    private Transform m_SelectedBottomParent;
    public List<GameObject> selectedBottomLeftButtons;
    public List<GameObject> selectedBottomMiddleButtons;
    public List<GameObject> selectedBottomRightButtons;


    private void Awake()
    {
        m_AnimatorNotSelected = transform.GetChild(0).GetComponent<Animator>();
        m_AnimatorSelected = transform.GetChild(1).GetComponent<Animator>();
        m_HudButtonPrefab = Resources.Load<GameObject>("Prefabs/Hud/HudButton");
        m_SelectedTopParent = m_AnimatorSelected.transform.GetChild(0).GetChild(1);
        m_SelectedBottomParent = m_AnimatorSelected.transform.GetChild(1).GetChild(1);
}

    private void Start()
    {
        SetupHud();
    }

    private void Update()
    {
        m_AnimatorNotSelected.SetBool("Visible", !ObjectIsSelected);
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
            Destroy(list[i].gameObject);
            #endif
        }

        list = new List<GameObject>();
    }

    public void SetupHud()
    {
        // Top Selected Panel
        SetupButtonsOnHud(HudSide.Left, ref selectedTopLeftButtons, manager.selectedPanelTopLeftButtons, m_SelectedTopParent);
        SetupButtonsOnHud(HudSide.Middle, ref selectedTopMiddleButtons, manager.selectedPanelTopMiddleButtons, m_SelectedTopParent);
        SetupButtonsOnHud(HudSide.Right, ref selectedTopRightButtons, manager.selectedPanelTopRightButtons, m_SelectedTopParent);

        // Bottom Selected Panel
        SetupButtonsOnHud(HudSide.Left, ref selectedBottomLeftButtons, manager.selectedPanelBotLeftButtons, m_SelectedBottomParent);
        SetupButtonsOnHud(HudSide.Middle, ref selectedBottomMiddleButtons, manager.selectedPanelBotMiddleButtons, m_SelectedBottomParent);
        SetupButtonsOnHud(HudSide.Right, ref selectedBottomRightButtons, manager.selectedPanelBotRightButtons, m_SelectedBottomParent);
    }
}
