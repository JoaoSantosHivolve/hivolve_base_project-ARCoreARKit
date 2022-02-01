using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PlacedObjectController : MonoBehaviour
{
    [Header("Settings Controller")]
    [Range(0.50f, 10f)] public float collisionWidth;
    [Range(0.15f, 2f)] public float collisionHeight;

    // Object components
    private GameObject m_ObjectHolder;
    private GameObject m_SelectionCanvas;
    private GameObject m_ScaleCanvas;
    private CapsuleCollider m_CapsuleCollider;

    private void Awake()
    {
        m_ObjectHolder = transform.GetChild(0).gameObject;
        m_SelectionCanvas = transform.GetChild(1).gameObject;
        m_ScaleCanvas = transform.GetChild(2).gameObject;
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        #if UNITY_EDITOR
        m_SelectionCanvas.transform.localScale = Vector3.one * collisionWidth;
        m_ScaleCanvas.transform.localPosition = new Vector3(0, collisionHeight / 2f, 0);
        m_CapsuleCollider.radius = collisionWidth * 0.1f;
        m_CapsuleCollider.height = collisionHeight / 2f;
        m_CapsuleCollider.center = new Vector3(0, collisionHeight / 4f, 0);
        #endif
    }

    public void SetObjectAnimation(bool selected)
    {
        m_ObjectHolder.transform.GetComponent<Animator>().SetBool("Selected", selected);
    }
    public void SetSelectionVisualizerColor(Color color)
    {
        m_SelectionCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = color;
    }
    public void SetSelectionVisualizerSprite(Sprite sprite)
    {
        m_SelectionCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
    }
    public void SetSelectionVisualizer(bool selected)
    {
        m_SelectionCanvas.transform.GetComponent<Animator>().SetBool("Selected",selected);
    }
    public void SetScaleVisualizer(bool scaling)
    {
        m_ScaleCanvas.transform.GetComponent<Animator>().SetBool("Scaling", scaling);
    }
    public void SetScaleVisualizerValue(float value)
    {
        m_ScaleCanvas.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((int)(value * 100)).ToString() + "%";
    }

    public ScaleManipulator GetScaleManipulator() => transform.parent.GetComponent<ScaleManipulator>();
    public RotationManipulator GetRotationManipulator() => transform.parent.GetComponent<RotationManipulator>();
}