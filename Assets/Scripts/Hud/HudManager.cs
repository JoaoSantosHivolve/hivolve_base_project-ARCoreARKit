using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        m_AnimatorNotSelected = transform.GetChild(0).GetComponent<Animator>();
        m_AnimatorSelected = transform.GetChild(1).GetComponent<Animator>();
    }

    private void Update()
    {
        m_AnimatorNotSelected.SetBool("Visible", !ObjectIsSelected);
        m_AnimatorSelected.SetBool("Visible", ObjectIsSelected);
    }
}
