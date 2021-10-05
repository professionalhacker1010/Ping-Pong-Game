using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//add this
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    #region
    private static LevelSelect _instance;
    public static LevelSelect Instance
    {
        get
        {
            if (_instance == null) Debug.Log("The LevelSelect is NULL");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public static bool firstGameStarted = false;

    /*[SerializeField] private List<GameObject> buttons; //references to all the buttons for enabling/disabling
    [SerializeField] private Button firstButton, backButton;

    public void LoadButtons() //call this every time you change to level select screen. Only loads already beaten opponents
    {
        for (int i = 0; i < LevelManager.currOpponent; i++)
        {
            buttons[i].SetActive(true);
        }
    }

    public void ChooseLevel(int i)
    {
        LevelManager.chosenOpponent = i;
        SceneManager.LoadScene("Game");
    }*/

    public IEnumerator TransitionToGame(int level)
    {
        if (level == 0) firstGameStarted = true;
        TransitionManager.Instance.QuickOut();
        yield return new WaitForSeconds(21 / 24f);
        LevelManager.chosenOpponent = level;
        SceneManager.LoadScene("Game");
    }

    /*public void LevelScreenButtonSelection()
    {
        if (buttons[0].activeSelf)
        {
            firstButton.Select();
        }
        else
        {
            backButton.Select();
        }
    }*/
}
