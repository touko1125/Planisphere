using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
            }

            return instance;
        }
    }

    [SerializeField]
    private bool IsDontDestroy = true;

    virtual protected void Awake()
    {
        if (!IsDontDestroy)
        {
            return;
        }

        if (this != Instance)
        {
            Destroy(gameObject);

            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}