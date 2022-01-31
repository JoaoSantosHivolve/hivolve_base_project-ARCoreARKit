using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    private List<GameObject> m_LoadingEffects;
    private int m_Index;

    private void Awake()
    {
        m_LoadingEffects = new List<GameObject>();

        for (int i = 0; i < transform.GetChild(2).childCount; i++)
        {
            var currChild = transform.GetChild(2).GetChild(i).gameObject;
            currChild.SetActive(false);
            m_LoadingEffects.Add(currChild);
        }

        // Set only selected active
        m_Index = ((int)AppManager.Instance.loadingType);
        m_LoadingEffects[m_Index].SetActive(true);

        // Start intro effect
        StartCoroutine(IntroTransition());
    }


    private IEnumerator IntroTransition()
    {
        var ended = false;
        var time = 0.0f;
        var animIntroDuration = m_LoadingEffects[m_Index].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animIntroDuration);

        while (!ended)
        {
            time = Mathf.Clamp(time + Time.deltaTime, 0, AppManager.Instance.loadingTime);

            // Line is selected
            if (m_Index == 1)
            {
                var value = time / AppManager.Instance.loadingTime;
                m_LoadingEffects[m_Index].GetComponent<Animator>().SetFloat("Time", value);
            }

            if (time >= AppManager.Instance.loadingTime)
            {
                ended = true;
            }

            yield return null;
        }

        m_LoadingEffects[m_Index].GetComponent<Animator>().SetBool("Ended", true);

        GetComponent<Animator>().SetBool("Ended", true);
    }
}
