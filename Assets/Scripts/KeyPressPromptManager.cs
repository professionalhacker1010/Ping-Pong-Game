using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPressPromptManager : MonoBehaviour
{
    #region
    private static KeyPressPromptManager _instance;
    public static KeyPressPromptManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("The KeyPressPromptManager is NULL");

            return _instance;
        }
    }
    #endregion

    [SerializeField] private List<KeyPressPrompt> keyPressPrompts;
    [SerializeField] private List<string> keyNames;
    private Dictionary<string, KeyPressPrompt> keyPressPromptStorage;

    private void Awake()
    {
        _instance = this;

        keyPressPromptStorage = new Dictionary<string, KeyPressPrompt>();
        for (int i = 0; i < keyNames.Count; i++) keyPressPromptStorage.Add(keyNames[i], keyPressPrompts[i]);
    }

    public KeyPressPrompt GetKeyPressPrompt(string keyName)
    {
        return keyPressPromptStorage[keyName];
    }
}
