using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameObjects;
    public static bool dataInitialized = false;
    void Awake()
    {
        if (!dataInitialized)
        {
            dataInitialized = true;
            DontDestroyOnLoad(this.gameObject);
            foreach (var item in gameObjects) DontDestroyOnLoad(item);
        }
        else
        {
            foreach (var item in gameObjects) Destroy(item);
            Destroy(this);
        }
    }
}
