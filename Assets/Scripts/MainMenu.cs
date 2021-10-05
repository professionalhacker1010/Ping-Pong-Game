using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//add this

public class MainMenu : MonoBehaviour
{
	private static bool saveStarted = false;

    private void Start()
    {
		TransitionManager.Instance.QuickIn();
    }

    public void PlayGame()
	{
		//play transition
		TransitionManager.Instance.QuickOut();

		//get the appropriate scene from start button
		if (!saveStarted) //condition for when first starting game
        {
			Debug.Log("saveStarted = " + saveStarted.ToString());
			saveStarted = true;
			StartCoroutine(ChangeScene("Cutscene"));
		}
		/*else if (GameManager.GameInProgress)
		{
			StartCoroutine(ChangeScene("Game"));
		}*/
		else if (CutsceneManager.cutsceneInProgress) //for if player exits to menu during cutscene and then goes back
        {
			StartCoroutine(ChangeScene("Cutscene"));
		}
		else
        {
			StartCoroutine(ChangeScene("LevelSelect"));
        }

		//Debug.Log("Game in prog: " + GameManager.GameInProgress.ToString());
		//Debug.Log("Cutscene in prog " + CutsceneManager.cutsceneInProgress.ToString());
	}

	//wait until transition is over to change scene
	private IEnumerator ChangeScene(string scene)
	{
		yield return new WaitForSeconds(21 / 24f);
		SceneManager.LoadScene(scene);
	}

	public void QuitGame()
	{
		//nothing will happen in the unity game view dw
		Application.Quit();
	}
}
