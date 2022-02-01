using UnityEngine;
using UnityEngine.UI;

public class HudButtonTutorial : HudButton
{
    private ArHintsManager m_HintsManager;

    protected override void Awake()
    {
        base.Awake();

        m_HintsManager = ArHintsManager.Instance;
    }

    protected override void Update()
    {
        base.Update();

        GetComponent<Button>().interactable = (m_HintsManager.AlreadyPlacedAnObject && !m_HintsManager.ObjectsArePlaced());
    }

    protected override void CheckIfActivated()
    {
        
    }

    protected override void OnClick()
    {
        m_HintsManager.AlreadyPlacedAnObject = false;
    }
}
