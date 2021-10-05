using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    #region
    private static DialogueManager _instance;
    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("The LevelSelectDialogueManager is NULL");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    //references to dialogue stuffs
    [SerializeField] private DialogueRunner dialogueRunner;

    // Start is called before the first frame update
    void Start()
    {
        //obtain current scene's yarn file
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
    }

    public void StartDialogue(YarnProgram file)
    {
        StartCoroutine(StartDialogueHelper(file));
    }

    private IEnumerator StartDialogueHelper(YarnProgram file)
    {
        dialogueRunner.Clear();
        yield return new WaitForSeconds(0.05f);
        dialogueRunner.Add(file);
        dialogueRunner.StartDialogue();
    }

    public bool DialogueRunning() {
        return dialogueRunner.IsDialogueRunning;
    }
}
