using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//add this
using UnityEngine.UI;
using Yarn.Unity;

public class PauseMenu : MonoBehaviour
{
	public static bool gameIsPaused = false;
	[SerializeField] private GameObject pauseMenuUI;
	[SerializeField] private Button firstButton;
	[SerializeField] private GameObject settingsUI;
	[SerializeField] private Button settingsFirstButton;
	[SerializeField] private Button levelSelectButton;
	[SerializeField] private GameObject pauseOverlay;

	[SerializeField] private DialogueRunner dialogueRunner;

    void Update()
	{
		if (KeyCodes.Pause() && SceneManager.GetActiveScene().name != "Menu" && !dialogueRunner.IsDialogueRunning && !TransitionManager.Instance.isTransitioning)
		{
			if (gameIsPaused)
			{
				Resume();
			}
			else
			{
				Pause();
			}
		}
	}

	private void Pause()
	{
		pauseMenuUI.SetActive(true);
		pauseOverlay.SetActive(true);
		if (SceneManager.GetActiveScene().name == "LevelSelect") levelSelectButton.interactable = false;
		else levelSelectButton.interactable = true;
		Time.timeScale = 0.0f;
		gameIsPaused = true;
		firstButton.Select();
	}

	public void Resume()
	{
		StartCoroutine(ResumeHelper());
	}

	private IEnumerator ResumeHelper()
    {
		yield return new WaitUntil(() => KeyCodes.HitGetUp() || KeyCodes.PauseGetUp());
		firstButton.FindSelectableOnDown().Select(); //this deselects the resume button so it will select again if you open the pause menu again
		pauseMenuUI.SetActive(false);
		pauseOverlay.SetActive(false);
		Time.timeScale = 1.0f;

		//doing this cause otherwise pressing Space to resume makes you also hit the paddle
		yield return new WaitForSeconds(0.1f);
		gameIsPaused = false;
	}

	public void EnableSettings()
    {
		settingsUI.SetActive(true);
		settingsFirstButton.Select();
		pauseMenuUI.SetActive(false);
    }

    public void DisableSettings() 
	{
		pauseMenuUI.SetActive(true);
		firstButton.Select();
		settingsUI.SetActive(false);
    }

    public void GoToLevelSelectScene()
	{
		StartCoroutine(ChangeScene("LevelSelect"));
	}

	public void GoToMenuScreen()
    {
		StartCoroutine(ChangeScene("Menu"));
	}

	//wait for transition to finish to change scene
	private IEnumerator ChangeScene(string scene)
	{
		yield return new WaitUntil(() => KeyCodes.HitGetUp());
		
		//play transition
		TransitionManager.Instance.QuickOut();
		yield return new WaitForSecondsRealtime(21 / 24f);
		Time.timeScale = 1.0f;

		//load scene
		SceneManager.LoadScene(scene);
		pauseMenuUI.SetActive(false);
		pauseOverlay.SetActive(false);

		//You don't have to restart if you exited from the YOU WIN screen!
		if (GameManager.Instance)
		{
			if (GameManager.Instance.GameIsWon())
			{
				LevelManager.currOpponent++; //THIS IS THE ONLY PLACE currOpponent CAN BE CHANGED!
			}
			GameManager.Instance.ResetGame();
		}
		gameIsPaused = false;

		//play transition
		TransitionManager.Instance.QuickIn();
	}
}
