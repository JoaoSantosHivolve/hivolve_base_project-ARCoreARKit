using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Debug.Log("Multiple Objects " + typeof(T).Name);
            Destroy(this.gameObject);
            return;
        }
    }
}

public class SingletonDestroyable<T> : MonoBehaviour where T : Component
{
    public static T Instance;

    public void InitializeDestroyableSingleton()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Debug.Log("Multiple Objects " + typeof(T).Name);
            Destroy(this.gameObject);
            return;
        }
    }
}
