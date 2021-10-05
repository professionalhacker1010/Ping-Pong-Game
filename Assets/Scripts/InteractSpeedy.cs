using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSpeedy : InteractCharacter
{
    private static bool hadEnergyDrink = false;
    protected override void Start()
    {
        base.Start();
        if (LevelManager.currOpponent == level) fileCounter = 1;
        if (hadEnergyDrink) table.UnlockThisTable();
    }

    protected override void HitReaction()
    {
        base.HitReaction();
    }

    protected override IEnumerator OnOutroDialogueComplete()
    {
        return base.OnOutroDialogueComplete();
    }

    protected override void StartDialogue()
    {
        //start dialogue
        if (LevelManager.currOpponent < level) //loops through same dialogue before you can play her
        {
            //DialogueManager.Instance.StartDialogue(preGameDialogue[0]);
            DialogueManager.Instance.StartDialogue(preGameDialogue[fileCounter]);
            if (fileCounter < preGameDialogue.Count - 1) fileCounter++;

        }
        else if (LevelManager.currOpponent == level) //true pregame dialogue starts when you can play her
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


}
