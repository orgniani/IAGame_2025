using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private bool isPersistent = true;

    private static T instance;

    public static T Instance 
    { 
        get
        {
            if (!instance)
                instance = FindFirstObjectByType<T>();

            return instance;
        } 
    }


    void Awake ()
    {
        if (Instance != this)
            Destroy(gameObject);
        else if (isPersistent)
            DontDestroyOnLoad(gameObject);
    }
}