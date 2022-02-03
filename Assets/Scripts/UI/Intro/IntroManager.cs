using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : SingletonDestroyable<IntroManager>
{
    private List<GameObject> m_LoadingEffects;
    private int m_Index;
    private AudioSource m_AudioSource;


    public void Init()
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

        // Get AudioSource
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.volume = AppManager.Instance.buttonVolume;
        m_AudioSource.clip = Resources.Load<AudioClip>("Sounds/Hud/Intro");

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

                // Play intro sound clip
                if(AppManager.Instance.playIntroSound)
                    m_AudioSource.Play();
            }

            yield return null;
        }

        m_LoadingEffects[m_Index].GetComponent<Animator>().SetBool("Ended", true);

        GetComponent<Animator>().SetBool("Ended", true);
    }
}