using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractShapeshifter : InteractCharacter
{
    [SerializeField] private List<GameObject> forms;
    [SerializeField] private GameObject poof;
    private Animator poofAnimator, shapeshiftAnimator, catAnimator;
    private int currForm = 0, catForm = 0;
    private static bool initialized = false;

    protected override void Start()
    {
        base.Start();
        if (!initialized)
        {
            dialogueRunner.AddCommandHandler("shapeshift", Shapeshift);
            dialogueRunner.AddCommandHandler("shapeshifterIntro", Intro);
            dialogueRunner.AddCommandHandler("shapeshifterOutro", Outro);
            initialized = true;
        }

        poofAnimator = poof.GetComponent<Animator>();
        shapeshiftAnimator = forms[0].GetComponent<Animator>();
        catAnimator = forms[1].GetComponent<Animator>();

        if (LevelManager.currOpponent <= level) //gonna have an intro where you interact with the shapeshifter's hitbox (but you think it's the table), then the shapeshifter poofs in, then you start the game
        {
            shapeshiftAnimator.SetTrigger("idle");

            //forms[0].SetActive(false);
            //transform.position = new Vector3(transform.position.x, 12f); //move out of the way so that the interact key prompt doesn't show up
        }
        else
        {
            shapeshiftAnimator.SetTrigger("idle");
            table.UnlockThisTable();
        }
    }

    //shapeshift stuffs
    #region
    public void Shapeshift(string[] parameters, System.Action onComplete)
    {
        StartCoroutine(ShapeshiftHelper(onComplete));
    }

    private IEnumerator ShapeshiftHelper(System.Action onComplete)
    {
        yield return new WaitForSeconds(0.5f);

        if (currForm == 0)
        {
            shapeshiftAnimator.SetTrigger("shapeshift"); //start shapeshift animation
            yield return new WaitForSeconds(22/24f); // wait for length of animation -3 frames
        }

        poof.SetActive(true);
        poofAnimator.SetTrigger("poof");
        yield return new WaitForSeconds(4 / 24f);

        if (currForm != 0 && currForm != forms.Count-1) //transition to shapeshifter inbetween forms
        {
            forms[currForm].SetActive(false);
            forms[0].SetActive(true);
            shapeshiftAnimator.SetTrigger("shapeshift");
            yield return new WaitForSeconds(22/24f);
            poofAnimator.SetTrigger("poof");
            yield return new WaitForSeconds(4 / 24f);
            forms[0].SetActive(false);
        }

        forms[currForm].SetActive(false);
        currForm++;

        if (currForm == forms.Count) currForm = 0;
        else if (currForm == 1) catForm = 0;

        forms[currForm].SetActive(true);
        if (currForm == 0) shapeshiftAnimator.SetTrigger("idle");
        yield return new WaitForSeconds(15 / 24f);

        poof.SetActive(false);
        onComplete();
    }

    public void Intro(string[] parameters, System.Action onComplete)
    {
        StartCoroutine(IntroHelper(onComplete));
    }

    private IEnumerator IntroHelper(System.Action onComplete)
    {
        yield return new WaitForSeconds(0.5f);

        poof.SetActive(true);
        poofAnimator.SetTrigger("poof");
        yield return new WaitForSeconds(4 / 24f);

        forms[0].SetActive(true);

        //chuckle 6 times
        for (int i = 0; i < 2; i++)
        {
            shapeshiftAnimator.SetTrigger("shapeshift");
            yield return new WaitForSeconds(15 / 24f);
            shapeshiftAnimator.SetTrigger("idle");
        }

        onComplete();
    }

    public void Outro(string[] parameters, System.Action onComplete)
    {
        StartCoroutine(OutroHelper(onComplete));
    }

    private IEnumerator OutroHelper(System.Action onComplete)
    {
        shapeshiftAnimator.SetTrigger("shapeshift");
        yield return new WaitForSeconds(22 / 24f);

        onComplete();
        StartCoroutine(TableSelectManager.Instance.TransitionToGame(level));
        yield return new WaitForSeconds(4 / 24f);

        shapeshiftAnimator.speed = 0f;
    }
    #endregion

    protected override void InteractionReaction()
    {
        //if (LevelManager.currOpponent >= level) //comment this out to read pregame dialogue early
        {
            base.InteractionReaction();
        }
    }

    protected override void HitReaction()
    {
        if (currForm == 1)
        {
            catForm++;
            if (catForm > 3)
            {
                catForm = 1;
                catAnimator.SetBool("3", false);
                catAnimator.SetBool("2", false);
                catAnimator.SetBool("1", false);
                catAnimator.SetTrigger("revert");
            }

            catAnimator.SetBool(catForm.ToString(), true);
        }
        else if (currForm == 2)
        {

        }
        else if (currForm == 3)
        {

        }
    }
}
