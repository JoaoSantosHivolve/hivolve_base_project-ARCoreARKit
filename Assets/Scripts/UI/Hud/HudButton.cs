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

            if(m_OnClip != null)
                GetComponent<AudioSource>().clip = value ? m_OnClip : m_OffClip;
        }
    }
    public HudButtonOnClickType type;
    private AudioClip m_OnClip;
    private AudioClip m_OffClip;

    protected virtual void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClickListener);
        Activated = false;
    }
    protected virtual void Update()
    {
        if(type == HudButtonOnClickType.Toggle)
        {
            CheckIfActivated();
        }
    }

    private void OnClickListener()
    {
        if (type == HudButtonOnClickType.Toggle)
        {
            Activated = !Activated;
        }

        GetComponent<AudioSource>().Play();

        OnClick();
    }

    protected abstract void OnClick();
    protected abstract void CheckIfActivated();

    public void SetColor(Color color)
    {
        GetComponent<Image>().color = color;
    }
    public void SetButtonVisibility(bool value)
    {
        GetComponent<Image>().enabled = value;
    }
    public void SetVolume(float volume)
    {
        GetComponent<AudioSource>().volume = volume;
    }
    public void SetAudioClip(AudioClip clip)
    {
        m_OnClip = clip;
        GetComponent<AudioSource>().clip = clip;
    }
    public void SetAudioClip(AudioClip onClip, AudioClip offClip)
    {
        m_OnClip = onClip;
        m_OffClip = offClip;

        GetComponent<AudioSource>().clip = m_OnClip;
    }
}