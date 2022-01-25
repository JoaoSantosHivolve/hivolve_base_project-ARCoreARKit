using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HudButtonOnClickType
{
    OneClick,
    Toggle
}

public abstract class HudButton : MonoBehaviour
{
    private Sprite m_DefaultSprite;
    public Sprite DefaultSprite
    {
        get { return m_DefaultSprite; }
        set 
        { 
            m_DefaultSprite = value;
            GetComponent<Image>().sprite = value;
        }
    }
    private Sprite m_ActivatedSprite;
    public Sprite ActivatedSprite
    {
        get { return m_ActivatedSprite; }
        set
        {
            m_ActivatedSprite = value;
            GetComponent<Image>().sprite = value;
        }
    }

    private bool m_Activated;
    protected bool Activated
    {
        get { return m_Activated; }
        set
        {
            m_Activated = value;

            GetComponent<Image>().sprite = value ? m_ActivatedSprite : DefaultSprite;
        }
    }
    public HudButtonOnClickType type;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(BaseOnClick);

        Activated = false;
    }
    private void Update()
    {
        if(type == HudButtonOnClickType.Toggle)
        {
            CheckIfActivated();
        }
    }

    private void BaseOnClick()
    {
        if (type == HudButtonOnClickType.Toggle)
        {
            Activated = !Activated;
        }

        OnClick();
    }
    protected abstract void OnClick();
    protected abstract void CheckIfActivated();

    public void SetColor(Color color)
    {
        GetComponent<Image>().color = color;
    }
}