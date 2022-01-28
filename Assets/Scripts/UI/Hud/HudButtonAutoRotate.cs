using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudButtonAutoRotate : HudButton
{
    protected override void CheckIfActivated()
    {
        if(ManipulationSystem.Instance.SelectedObjectManager != null)
        {
            Activated = ManipulationSystem.Instance.SelectedObjectManager.GetRotationManipulator().autoRotate;
        }
    }

    protected override void OnClick()
    {
        ManipulationSystem.Instance.SelectedObjectManager.GetRotationManipulator().autoRotate = Activated;
    }
}
