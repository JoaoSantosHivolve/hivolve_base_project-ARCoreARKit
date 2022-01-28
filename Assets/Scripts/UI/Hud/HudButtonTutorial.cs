using UnityEngine;
using UnityEngine.UI;

public class HudButtonTutorial : HudButton
{
    private ArHintsBehaviour m_Hints;

    protected override void Awake()
    {
        base.Awake();

        m_Hints = GameObject.Find("Ar Hints").GetComponent<ArHintsBehaviour>();
    }

    protected override void Update()
    {
        base.Update();

        GetComponent<Button>().interactable = (m_Hints.AlreadyPlacedAnObject && !m_Hints.ObjectsArePlaced());
    }

    protected override void CheckIfActivated()
    {
        
    }

    protected override void OnClick()
    {
        m_Hints.AlreadyPlacedAnObject = false;
    }
}
