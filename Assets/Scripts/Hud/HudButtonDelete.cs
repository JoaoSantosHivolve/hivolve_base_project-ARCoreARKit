using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudButtonDelete : HudButton
{
    protected override void OnClick()
    {
        ArManager.Instance.DeleteSelectedObject();
    }

    private void Start()
    {
        m_OnClickType = HudButtonOnClickType.OneClick;
    }
}