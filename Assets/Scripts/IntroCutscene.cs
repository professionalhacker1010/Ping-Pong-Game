using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCutscene : MonoBehaviour
{
    //for intro cutscene
    [SerializeField] private int panFrames;
    [SerializeField] private float friedItemLocation;
    [SerializeField] private BoxCollider2D doorSlamBoxCollider, introCutsceneTriggerBoxCollider;
    [SerializeField] private CharacterControls playerControls;
    [SerializeField] private CameraFollower cameraFollower;
    [SerializeField] private GameObject introFenceCollider, door, cameraObject;
    [SerializeField] private YarnProgram yarnFile;
    [SerializeField] private Vector3 spacePromptPosition;
    private KeyPressPrompt spacePrompt;

    // Start is called before the first frame update
    void Start()
    {
        spacePrompt = KeyPressPromptManager.Instance.GetKeyPressPrompt("space");
        spacePrompt.SetConditions(() => playerControls.OverlapsRightHitBox(doorSlamBoxCollider) || playerControls.OverlapsLeftHitBox(doorSlamBoxCollider), spacePromptPosition);

        if (!TableSelectManager.firstGameStarted) StartCoroutine(IntroCutScene());
        else Destroy(this.gameObject);
    }

    private IEnumerator IntroCutScene()
    {
        cameraFollower.LockNormalBehavior();
        door.SetActive(false);
        yield return new WaitUntil(() => playerControls.OverlapsRightHitBox(doorSlamBoxCollider) && KeyCodes.Hit() && !PauseMenu.gameIsPaused);

        spacePrompt.RemoveConditions();
        door.SetActive(true);
        introFenceCollider.SetActive(false);
        StartCoroutine(cameraFollower.PanToPlayer(15));
        cameraFollower.UnlockNormalBehaviour();

        yield return new WaitUntil(() => playerControls.OverlapsHitBox(introCutsceneTriggerBoxCollider));

        playerControls.LockCharacterControls();
        cameraFollower.LockNormalBehavior();
        cameraFollower.StopCurrentPan();

        //pan to fried item
        yield return cameraFollower.PanCamera(friedItemLocation, panFrames);

        yield return new WaitForSeconds(2.0f); //linger on fried item for a while

        //pan back to player
        yield return cameraFollower.PanToPlayer(panFrames);

        yield return new WaitForSeconds(1.0f);

        DialogueManager.Instance.StartDialogue(yarnFile); //to do: wait for player to make some determined expression

        playerControls.UnlockCharacterControls();
        cameraFollower.UnlockNormalBehaviour();
        cameraFollower.UnlockNormalBehaviour();
    }


}
