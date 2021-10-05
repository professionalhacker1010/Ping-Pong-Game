using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("The LevelManager is NULL");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    [SerializeField] private List<Opponent> opponents;
    [SerializeField] private List<GameObject> opponentObjects;
    public static int currOpponent = 0; //current opponent in game's progression, just to keep track
    public static int prevOpponent = 0; //previous opponent played, whether it be from game's prog. or level select screen
    public static int chosenOpponent = 0; //opponent chosen from level select screen or from game's progression

    //for simple round # version
    [SerializeField] private List<Sprite> numbers;
    [SerializeField] private UnityEngine.UI.Image number;
    [SerializeField] private GameObject roundNum;

    [SerializeField] private PaddleControls playerPaddle;

    private void Start()
    {
        playerPaddle.LockInputs();
        if (GameObject.FindWithTag("PersistentData") != null) StartCoroutine(RoundNumberIntro());
        TransitionManager.Instance.QuickIn();

       ChooseOpponent(chosenOpponent); //turn on/off appropriate gameObjects
    }

    public Opponent GetChosenOpponent()
    {
        opponentObjects[chosenOpponent].SetActive(true);
        return opponents[chosenOpponent];
    }

    public void ChooseOpponent(int i) //use this function for changing opps in level select screen
    {
        opponentObjects[prevOpponent].SetActive(false);
        prevOpponent = i;
        chosenOpponent = i;
        if (!opponentObjects[chosenOpponent].activeInHierarchy) opponentObjects[chosenOpponent].SetActive(true);
    }

    //lock paddle inputs while transition is playing - INTRO CARD VERSION
    public IEnumerator TransitionAndUnlock()
    {
        TransitionManager.Instance.QuickIn();
        yield return new WaitForSeconds(21 / 24f);
        playerPaddle.UnlockInputs();
    }

    //ROUND# INTRO VERSION
    private IEnumerator RoundNumberIntro()
    {
        yield return new WaitForSeconds(0.25f);

        //Debug.Log("round number shown");
        number.sprite = numbers[LevelManager.currOpponent+1];
        roundNum.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        roundNum.SetActive(false);
        yield return new WaitForSeconds(0.25f);

        StartCoroutine(TransitionAndUnlock());
    }
}
