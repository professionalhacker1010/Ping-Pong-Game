using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCharacter : Interact
{
    //yarn files
    [SerializeField] protected List<YarnProgram> preGameDialogue;
    [SerializeField] protected List<YarnProgram> postGameDialogue;
    protected int fileCounter = 0;
    [SerializeField] protected bool turnsToPlayer, facingLeft;

    //opponent info
    [SerializeField] protected TableSelect table;
    [SerializeField] protected int level;
    protected static bool outroDialoguePlayed = false; //automatically trigger outro dialogue when first beating the opponent

    //interact key prompt is always a C above their heads
    private KeyPressPrompt cKeyPrompt;
    private bool cKeyPromptSet = false;
    private float cKeyPromptHeight = 3.0f;

    float minDistance = 2.5f; //not serializable cause I don't wanna adjust individually for each character lol

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (LevelManager.currOpponent <= level) table.LockThisTable(); //when curr opponent == level, then determine when to unlock table in custom child script
        if (!outroDialoguePlayed && LevelManager.currOpponent == level+1)
        {
            InteractionReaction();
            StartCoroutine(OnOutroDialogueComplete());
        }

        cKeyPrompt = KeyPressPromptManager.Instance.GetKeyPressPrompt("C");
    }

    protected override void OverlapPlayerColliderBehaviour()
    {
        base.OverlapPlayerColliderBehaviour();
        //only set the key prompt conditions on first entering interactable distance
        if (!cKeyPromptSet)
        {
            cKeyPrompt.SetConditions(() => (characterControls.OverlapsLeftHitBox(thisCollider) || characterControls.OverlapsRightHitBox(thisCollider)) && !DialogueManager.Instance.DialogueRunning(), 
                new Vector3(transform.position.x, transform.position.y + cKeyPromptHeight));
            StartCoroutine(OnNotOverlapPlayerCollider());
        }
    }

    private IEnumerator OnNotOverlapPlayerCollider()
    {
        yield return new WaitUntil(() => !characterControls.OverlapsLeftHitBox(thisCollider) && !characterControls.OverlapsRightHitBox(thisCollider));

        cKeyPromptSet = false;
    }

    //no dialogue, just visual and audio reactions
    protected override void HitReaction()
    {
        base.HitReaction();
    }

    //before playing for the first time, cycles through before game dialogue. When exhausted, repeats the last file.
    //after playing, cycles through post game dialogue. When exhausted, repeats the last file.
    protected override void InteractionReaction()
    {
        float distance = transform.position.x - characterControls.transform.position.x;

        //turn to face player
        if (turnsToPlayer)
        {
            if ((distance >= 0 && !facingLeft) || (distance < 0 && facingLeft))
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
                StartCoroutine(FlipBackOnDialogueComplete());
            }
        }

        //check if player overlaps speaker awkwardly and readjust -- then start dialogue after readjustment
        if (Mathf.Abs(distance) < minDistance)
        {
            StartCoroutine(ReadjustPlayer(distance));
        }
        else StartDialogue();

    }

    protected virtual IEnumerator OnOutroDialogueComplete()
    {
        yield return new WaitUntil(() => DialogueManager.Instance.DialogueRunning());
        yield return new WaitUntil(() => !DialogueManager.Instance.DialogueRunning());
        outroDialoguePlayed = true;
    }

    private IEnumerator ReadjustPlayer(float distance)
    {
        float horizontalAxis;
        if (distance >= 0f) //face right and move until reaching -minDist
        {
            characterControls.FaceRight();
            horizontalAxis = -1f;
        }
        else // face left and move until reaching +minDist
        {
            characterControls.FaceLeft();
            horizontalAxis = 1f;
        }

        characterControls.characterPositionAdjustment.enabled = true;
        characterControls.characterPositionAdjustment.InitializeAdjustment(horizontalAxis);
        while (Mathf.Abs(transform.position.x - characterControls.transform.position.x) < minDistance)
        {
            yield return new WaitForSeconds(1 / 60f);
        }

        characterControls.characterPositionAdjustment.enabled = false; //set idle
        StartDialogue();
    }

    protected virtual void StartDialogue()
    {
        //start dialogue
        if (LevelManager.currOpponent <= level)
        {
            DialogueManager.Instance.StartDialogue(preGameDialogue[fileCounter]);
            if (fileCounter < preGameDialogue.Count - 1) fileCounter++;
        }
        else
        {
            DialogueManager.Instance.StartDialogue(postGameDialogue[fileCounter]);
            if (fileCounter < postGameDialogue.Count - 1) fileCounter++;
        }
    }

    private IEnumerator FlipBackOnDialogueComplete()
    {
        yield return new WaitUntil(() => dialogueRunner.Dialogue.IsActive);
        yield return new WaitUntil(() => !dialogueRunner.Dialogue.IsActive);
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    
}
