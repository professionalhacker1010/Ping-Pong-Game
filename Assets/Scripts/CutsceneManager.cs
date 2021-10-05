using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;//add this

public class CutsceneManager : MonoBehaviour
{
    #region
    private static CutsceneManager _instance;
    public static CutsceneManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("The CutsceneManager is NULL");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    [SerializeField] private List<YarnProgram> cutscenes;
    public static int currCutscene = 0; //0 = intro
    public static bool cutsceneInProgress = true;
    //private bool dialogueStarted = false;
    private DialogueRunner dialogueRunner;

    private void Start()
    {
        cutsceneInProgress = true;
        Debug.Log("Currcutscene: " + currCutscene.ToString());
        //obtain current cutscene's yarn file
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();

        //play transition animation
        if (currCutscene == 0)
        {
            TransitionManager.Instance.StartIn();
        }
        else
        {
            TransitionManager.Instance.QuickIn();
        }

        //start dialogue
        dialogueRunner.Add(cutscenes[currCutscene]);
        dialogueRunner.StartDialogue();
    }

    //wait for transition to finish to change to game scene
    public IEnumerator ChangeScene()
    {
        TransitionManager.Instance.QuickOut();
        yield return new WaitForSeconds(21 / 24f);
        currCutscene++; //make sure current cutscene is incremented for next time
        cutsceneInProgress = false;
        //SceneManager.LoadScene("Game");
        SceneManager.LoadScene("LevelSelect");
    }
}
