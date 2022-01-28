using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudButtonDelete : HudButton
{
    protected override void CheckIfActivated()
    {
    }

    protected override void OnClick()
    {
        ArManager.Instance.DeleteSelectedObject();
    }
}