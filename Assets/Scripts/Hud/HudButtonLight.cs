using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudButtonLight : HudButton
{
    private GameObject m_ExtraLightsHolder;

    protected override void CheckIfActivated()
    {
        
    }

    protected override void OnClick()
    {
        m_ExtraLightsHolder.SetActive(Activated);
    }

    private void Start()
    {
        m_ExtraLightsHolder = GameObject.Find("Lights");
    }
}
