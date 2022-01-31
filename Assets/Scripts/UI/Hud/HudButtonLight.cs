using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudButtonLight : HudButton
{
    protected override void CheckIfActivated()
    {
        Activated = AppManager.Instance.m_ExtraLights.activeSelf;
    }

    protected override void OnClick()
    {
        AppManager.Instance.SetExtraLights(Activated);
    }
}