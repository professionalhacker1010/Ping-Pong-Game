using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//add this


public class GameManager : MonoBehaviour
{
    //singleton stuff
    #region
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("The GameManager is NULL");

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    [SerializeField] private bool DEBUG = false;
    [SerializeField] private GameObject gameLost, gameWon, spaceToCont;
    [SerializeField] public int winRounds;
    [HideInInspector] public int playerWins = 0, playedRounds = 0, opponentWins = 0;
    public static bool GameInProgress;

    private void Start()
    {
        GameInProgress = true;
        Debug.Log("Game Started");
    }

    private void Update()
    {
        if (DEBUG && Input.GetKeyDown(KeyCode.Space))
        {
            LevelManager.currOpponent++;
            TransitionManager.Instance.QuickOut();
            SceneManager.LoadScene("LevelSelect");
        }
    }

    public void GameOver()
    {
        if (GameIsWon())
        {
            GameInProgress = false;
            gameWon.SetActive(true);
            spaceToCont.SetActive(true);
        }
        else if (GameIsLost())
        {
            gameLost.SetActive(true);
            //try again or exit to menu
            spaceToCont.SetActive(true);
        }
    }

    public void AddOpponentWin()
    {
        opponentWins++;
        playedRounds++;
        ScoreDatabase.Instance.addOpponentScoreboard();
        //Debug.Log("OPPONENT WINS: " + opponentWins.ToString());
    }

    public void AddPlayerWin()
    {
        playerWins++;
        playedRounds++;
        ScoreDatabase.Instance.addPlayerScoreboard();
       // Debug.Log("PLAYER WINS: " + playerWins.ToString());
    }

    //use these checks in some UI script that manages the control access and animations
    public bool GameIsWon()
    {
        return playerWins == winRounds;
    }

    public bool GameIsLost()
    {
        return opponentWins == winRounds;
    }

    public void ResetGame()
    {
        playerWins = 0;
        playedRounds = 0;
        opponentWins = 0;
    }

    public int PlayedRounds()
    {
        return playedRounds;
    }
}
