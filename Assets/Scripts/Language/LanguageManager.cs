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

public class LanguageManager : Singleton<LanguageManager>
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

    //public Dropdown dropdown;

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

        //dropdown.value = value;
    }
}
