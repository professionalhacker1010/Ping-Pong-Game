using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractKevin : InteractCharacter
{
    [SerializeField] Vector2 watchingVideosPosition;
    private static bool watchingVideos = false;

    //For kevin, make tables not selectable until you talk to him for the first time
    //after you beat him and the outro dialogue plays, he will sit in the corner watching youtube videos for the rest of the game
    protected override void Start()
    {
        base.Start();
        if (watchingVideos) transform.position = watchingVideosPosition;
    }

    protected override void InteractionReaction()
    {
        base.InteractionReaction();

        if (fileCounter == 1 && level <= LevelManager.currOpponent)
        {
            table.UnlockThisTable();
        }
    }

    protected override IEnumerator OnOutroDialogueComplete()
    {
        yield return base.OnOutroDialogueComplete();
        //move kevin to the corner
        transform.position = watchingVideosPosition;
        watchingVideos = true;
    }
}
