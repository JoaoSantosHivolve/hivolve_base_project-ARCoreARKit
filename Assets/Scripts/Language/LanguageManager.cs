using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Language
{
    Portugues,
    English,
    French,
    German,
    Spanish
}

public class LanguageManager : SingletonDestroyable<LanguageManager>
{
    private Language m_Language;
    public Language Language
    {
        get => m_Language;
        set
        {
            m_Language = value;

            var components = Resources.FindObjectsOfTypeAll<SetLanguageOnText>();

            foreach (var item in components)
            {
                item.UpdateText();
            }
        }
    }

    private Animator m_LanguagePanel;
    private Button m_PanelCloseButton;

    public void Init()
    {
        Language = AppManager.Instance.appLanguage;
        m_LanguagePanel = GameObject.Find("Language Panel").GetComponent<Animator>();
        m_PanelCloseButton = m_LanguagePanel.transform.GetChild(2).GetComponent<Button>();
        m_PanelCloseButton.onClick.AddListener(ClosePanel);
    }

    private void ClosePanel() => SetPanelVisibility(false);
    public void SetPanelVisibility(bool value) => m_LanguagePanel.SetBool("Enter", value);

    public void SetLanguage(int value)
    {
        switch (value)
        {
            case 0:
                Language = Language.Portugues;
                break;
            case 1:
                Language = Language.English;
                break;
            case 2:
                Language = Language.French;
                break;
            case 3:
                Language = Language.German;
                break;
            case 4:
                Language = Language.Spanish;
                break;
            default:
                break;
        }
    }
}