using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PlacedObjectManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(0.50f, 10f)] public float collisionWidth;
    [Range(0.15f, 2f)] public float collisionHeight;

    [Header("Components")]
    [SerializeField] private GameObject m_SelectionCanvas;
    [SerializeField] private GameObject m_ScaleCanvas;
    [SerializeField] private CapsuleCollider m_CapsuleCollider;

    private void Awake()
    {
        m_SelectionCanvas = transform.GetChild(1).gameObject;
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
        m_SelectionCanvas.transform.GetChild(0).GetComponent<Animator>().SetBool("Selected",selected);
    }

}
