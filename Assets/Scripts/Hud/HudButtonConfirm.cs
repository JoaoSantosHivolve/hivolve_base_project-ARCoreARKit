using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudButtonConfirm : HudButton
{
    protected override void OnClick()
    {
        ArManager.Instance.DeselectObject();
    }

    private void Start()
    {
        m_OnClickType = HudButtonOnClickType.OneClick;
    }
}
