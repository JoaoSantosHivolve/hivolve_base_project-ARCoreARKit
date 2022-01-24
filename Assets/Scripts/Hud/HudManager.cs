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
    private Transform m_SelectedPanelParent;
    public ArManager manager;
    public List<GameObject> leftSideButtons;
    public List<GameObject> middleSideButtons;
    public List<GameObject> rightSideButtons;

    private void Awake()
    {
        m_AnimatorNotSelected = transform.GetChild(0).GetComponent<Animator>();
        m_AnimatorSelected = transform.GetChild(1).GetComponent<Animator>();
        m_HudButtonPrefab = Resources.Load<GameObject>("Prefabs/Hud/HudButton");
        m_SelectedPanelParent = m_AnimatorSelected.transform.GetChild(0).GetChild(1);
    }

    private void Update()
    {
        m_AnimatorNotSelected.SetBool("Visible", !ObjectIsSelected);
        m_AnimatorSelected.SetBool("Visible", ObjectIsSelected);
    }

    //private void SetupHud()
    //{
    //    // ----- Set "object selected" panel buttons
    //    // --- Left side
    //    // Start with a clean state
    //    DeleteButtonsOnList(ref leftSideButtons);
    //    // Cant add more than 3 buttons
    //    if (manager.leftSideButtons.Count > 3)
    //    {
    //        Debug.Log("Left side buttons count > 3");
    //        return;
    //    }
    //    // Instantiate buttons
    //    foreach (var item in manager.leftSideButtons)
    //    {
    //        // Instantiate button
    //        var newButton = Instantiate(m_HudButtonPrefab, m_SelectedPanelParent);
    //        // Set name according to type of button
    //        newButton.name = "HudButton - " + item.ToString();
    //        // Add type of button script
    //        AddButtonScript(newButton, item);
    //        // Set correct color
    //        newButton.GetComponent<HudButton>().SetColor(manager.hudColor);
    //        // Set button sprites
    //        AddButtonSprites(newButton.GetComponent<HudButton>(), item);
    //        // Add button to list
    //        leftSideButtons.Add(newButton);
    //    }
    //    // Position buttons
    //    var count = leftSideButtons.Count;
    //    var size = manager.buttonsSize;
    //    var remainingSpace = 1f - (size * count);
    //    var increment = remainingSpace / (2 + (count - 1));
    //    for (int i = 0; i < count; i++)
    //    {
    //        var maxY = 1f - ((increment + (increment * i)) + (i * size));
    //        var minY = maxY - size;
    //        leftSideButtons[i].GetComponent<RectTransform>().anchorMin = new Vector2(leftSideAnchorPos, minY);
    //        leftSideButtons[i].GetComponent<RectTransform>().anchorMax = new Vector2(leftSideAnchorPos, maxY);
    //    }
    //}
    public void SetupSelectedObjectHud(HudSide side, ref List<GameObject> panelButtons, List<HudButtonType> buttonsToAdd)
    {
        // ----- Set "object selected" panel buttons
        // --- Left side
        // Start with a clean state
        DeleteButtonsOnList(ref panelButtons);
        // Cant add more than 3 buttons
        if (buttonsToAdd.Count > 3)
        {
            Debug.Log("Buttons count > 3");
            return;
        }
        // Instantiate buttons
        foreach (var item in buttonsToAdd)
        {
            // Instantiate button
            var newButton = Instantiate(m_HudButtonPrefab, m_SelectedPanelParent);
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
                break;
            case HudButtonType.AutoRotate:
                button.AddComponent<HudButtonAutoRotate>();
                break;
            case HudButtonType.Zoom:
                button.AddComponent<HudButtonZoom>();
                break;
            case HudButtonType.Confirm:
                button.AddComponent<HudButtonConfirm>();
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
                break;
            case HudButtonType.Zoom:
                break;
            case HudButtonType.Confirm:
                button.DefaultSprite = Resources.Load<Sprite>(path + "Confirm");
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

    [ContextMenu("UPDATE HUD")]
    public void SetupHudViaInspector()
    {
        SetupSelectedObjectHud(HudSide.Left, ref leftSideButtons, manager.selectedPanelLeftButtons);
        SetupSelectedObjectHud(HudSide.Middle, ref middleSideButtons, manager.selectedPanelMiddleButtons);
        SetupSelectedObjectHud(HudSide.Right, ref rightSideButtons, manager.selectedPanelRightButtons);
    }
}
