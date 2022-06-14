using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Instancable<T> : MonoBehaviour where T : class
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;
            }
            return instance;
        }
    }
}

public abstract class InstancableNB<T> : NetworkBehaviour where T : class
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;
            }
            return instance;
        }
    }
}