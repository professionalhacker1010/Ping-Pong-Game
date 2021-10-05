using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//add this

public class ButtonMash : MonoBehaviour
{
    //button mash
    [Header("Button Mash")]
    [SerializeField] private RectTransform buttonMashBar;
    [SerializeField] GameObject buttonMash, gameCrash, glitchOutAnimation;
    [SerializeField] private float maxPoints;[SerializeField] private float pointsDecayPerSecond, startingPoints, pointsPerMashEasy, pointsPerMashMedium, pointsPerMashHard; //button mash bar
    [SerializeField] private float mediumModePoints, hardModePoints; //when to trigger stuff based on how filled the bar is
    [SerializeField] private float maxBarScaledHeight;
    [SerializeField] private YarnProgram outroDialogue;
    private bool buttonMashBroken = false;
    private float points, decay, barUnit;

    void Awake()
    {
        points = startingPoints;
        buttonMash.SetActive(true);
        decay = pointsDecayPerSecond / 60;
        barUnit = maxBarScaledHeight / maxPoints;
        buttonMashBar.sizeDelta = new Vector2(buttonMashBar.sizeDelta.x, barUnit * points);
        StartCoroutine(Decay());
    }

    private void Update()
    {
        if (!buttonMashBroken)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (points >= hardModePoints) points += pointsPerMashHard; //add points
                else if (points >= mediumModePoints) points += pointsPerMashMedium;
                else points += pointsPerMashEasy;
            }
            //update UI and effects
            buttonMashBar.sizeDelta = new Vector2(buttonMashBar.sizeDelta.x, barUnit * points);

            if ((points >= maxPoints || points <= 0)) //trigger game crashing no matter the outcome
            {
                buttonMashBroken = true;
                StartCoroutine(CrashGame());
            }
        }
    }

    private IEnumerator CrashGame()
    {
        print("crash game");
        gameCrash.SetActive(true);
        glitchOutAnimation.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        glitchOutAnimation.SetActive(false);
        buttonMash.SetActive(false);
        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.StartDialogue(outroDialogue);
        yield return new WaitUntil(() => DialogueManager.Instance.DialogueRunning());
        yield return new WaitUntil(() => !DialogueManager.Instance.DialogueRunning());

        TransitionManager.Instance.QuickOut();
        yield return new WaitForSeconds(21 / 24f);

        SceneManager.LoadScene("LevelSelect");
        LevelManager.currOpponent++; //THIS IS THE ONLY PLACE currOpponent CAN BE CHANGED!
    }

    private IEnumerator Decay()
    {
        while (!buttonMashBroken)
        {
            points -= decay;
            yield return new WaitForSeconds(1 / 60f);
        }
    }
}
