using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetLanguageOnText : MonoBehaviour
{
    [TextArea(12, 12)]
    public string portugues;
    [TextArea(12, 12)]
    public string english;
    [TextArea(12, 12)]
    public string french;
    [TextArea(12, 12)]
    public string german;
    [TextArea(12, 12)]
    public string spanish;

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        var text = "";
#if UNITY_EDITOR
        var language = GameObject.FindObjectOfType<LanguageManager>().Language;
#else
        var language = LanguageManager.Instance.Language;
#endif
        switch (language)
        {
            case Language.Portugues:
                text = portugues;
                break;
            case Language.English:
                text = english;
                break;
            case Language.French:
                text = french;
                break;
            case Language.German:
                text = german;
                break;
            case Language.Spanish:
                text = spanish;
                break;
            default:
                break;
        }
        GetComponent<TextMeshProUGUI>().text = text;
    }
}
