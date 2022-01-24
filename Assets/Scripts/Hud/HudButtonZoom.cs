using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudButtonZoom : HudButton
{
    protected override void OnClick()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        m_OnClickType = HudButtonOnClickType.Toggle;
    }
}
