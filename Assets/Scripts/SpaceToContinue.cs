using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//add this

public class SpaceToContinue : MonoBehaviour
{
    void Update()
    {
        if (KeyCodes.Hit())
        {
            //StartCoroutine(ChangeScene("LevelSelect")); //debug

            if (GameManager.Instance.GameIsWon())
            {
                //if you've beaten the next highest level
                if (LevelManager.currOpponent == LevelManager.chosenOpponent)
                {
                    //StartCoroutine(ChangeScene("Cutscene"));
                    StartCoroutine(ChangeScene("LevelSelect"));
                }
                //if you've beaten a level you've already beaten
                else
                {
                    //StartCoroutine(ChangeScene("Menu"));
                    StartCoroutine(ChangeScene("LevelSelect"));
                }
                GameManager.Instance.ResetGame();
            }
            else if (GameManager.Instance.GameIsLost())
            {
                StartCoroutine(ChangeScene("Game"));
            }
        }
    }

    //wait for transition to finish to change scene
    public IEnumerator ChangeScene(string scene)
    {
        TransitionManager.Instance.QuickOut();
        yield return new WaitForSeconds(21 / 24f);
        SceneManager.LoadScene(scene);

        //if (scene == "Cutscene")
        if (scene == "LevelSelect")
        {
            LevelManager.currOpponent++; //THIS IS THE ONLY PLACE currOpponent CAN BE CHANGED!        
        }
    }
}
