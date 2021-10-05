using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Interact : MonoBehaviour
{
    //player colliders
    protected CharacterControls characterControls;

    //this object's collider
    protected Collider2D thisCollider;

    //this object's sprite renderer
    protected SpriteRenderer spriteRenderer;

    protected DialogueRunner dialogueRunner;

    //key press prompt is same for all interactables? idk yet, so code for that is in intereactCharacter for now

    //to do: sound effects

    protected virtual void Start()
    {
        characterControls = FindObjectOfType<CharacterControls>();
        thisCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    protected virtual void Update()
    {
        if (characterControls.OverlapsLeftHitBox(thisCollider) || characterControls.OverlapsRightHitBox(thisCollider))
        {
            OverlapPlayerColliderBehaviour();
        }
    }
    
    protected virtual void OverlapPlayerColliderBehaviour()
    {
        TableSelectManager.Instance.LockSelection();
        StartCoroutine(SelectableOnDialogueEnd());
        if (!dialogueRunner.IsDialogueRunning)
        {
            if (KeyCodes.Hit())
            {
                //trigger interaction
                HitReaction();
            }

            if (KeyCodes.Interact())
            {
                //trigger interaction
                InteractionReaction();
            }
        }
    }

    protected virtual void HitReaction()
    {

    }

    protected virtual void InteractionReaction()
    {

    }

    private IEnumerator SelectableOnDialogueEnd()
    {
        yield return new WaitUntil ( () => !characterControls.OverlapsLeftHitBox(thisCollider) && !characterControls.OverlapsRightHitBox(thisCollider) && !dialogueRunner.Dialogue.IsActive);
        TableSelectManager.Instance.UnlockSelection();
    }
}
