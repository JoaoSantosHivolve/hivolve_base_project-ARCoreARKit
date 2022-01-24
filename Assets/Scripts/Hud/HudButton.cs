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

    private Image m_Image;
    protected HudButtonOnClickType m_OnClickType;

    private void Awake()
    {
        m_Image = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(BaseOnClick);
    }
    private void BaseOnClick()
    {

        OnClick();
    }
    protected abstract void OnClick();

    public void SetColor(Color color)
    {
        GetComponent<Image>().color = color;
    }
}