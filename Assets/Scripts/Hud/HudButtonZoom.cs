using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudButtonZoom : HudButton
{
    protected override void CheckIfActivated()
    {
        if (ManipulationSystem.Instance.SelectedObjectManager != null)
        {
            //Activated = ManipulationSystem.Instance.SelectedObjectManager.GetScaleManipulator().autoRotate;
        }
    }

    protected override void OnClick()
    {
        //ManipulationSystem.Instance.SelectedObjectManager.GetScaleManipulator().autoRotate = Activated;
    }

    private void Start()
    {
        m_OnClickType = HudButtonOnClickType.Toggle;
    }
}
