using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControls : MonoBehaviour
{
    //for movement
    [SerializeField] private float normalSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator body, arm, swipe;
    [SerializeField] private SpriteRenderer armSpriteRenderer;
    [SerializeField] private GameObject rightHitBoxObject, leftHitBoxObject;
    [SerializeField] public CharacterPositionAdjustment characterPositionAdjustment;
    private BoxCollider2D rightHitBox, leftHitBox, hitBox;
    private bool movementHalted;

    private void Awake()
    {
        rightHitBox = rightHitBoxObject.GetComponent<BoxCollider2D>();
        leftHitBox = leftHitBoxObject.GetComponent<BoxCollider2D>();
        hitBox = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        //spawn at the correct table, only starts when you've played your first game
        if (TableSelectManager.firstGameStarted) transform.position = new Vector3(TableSelectManager.Instance.TableTransformX()-1.0f, transform.position.y);
    }

    private void Update()
    {
        // move paddle with keyboard
        float horizontalInput = Input.GetAxis("Horizontal");

        //get keycode to set direction of animations
        if (KeyCodes.Left())
        {
            FaceLeft();
        }
        else if (KeyCodes.Right())
        {
            FaceRight();
        }

        //move character at correct velocity
        if (!movementHalted && !DialogueManager.Instance.DialogueRunning())
        {
            MoveCharacter(horizontalInput, normalSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
            body.SetFloat("velocity", 0f);
            arm.SetFloat("velocity", 0f);
        }

        if (KeyCodes.Hit() && !DialogueManager.Instance.DialogueRunning() && !PauseMenu.gameIsPaused)
        {
            swipe.SetTrigger("swipe");
            StopAllCoroutines();
            StartCoroutine(HideArms());
        }
    }

    public void MoveCharacter(float horizontalInput)
    {
        MoveCharacter(horizontalInput, normalSpeed);
    }

    private void MoveCharacter(float horizontalInput, float speed)
    {
        // velocity calculation method to prevent bugginess while running into walls
        float velocity = horizontalInput * speed;
        rb.velocity = new Vector2(velocity, 0.00f);
        body.SetFloat("velocity", velocity);
        arm.SetFloat("velocity", velocity);

        if (horizontalInput == 0)
        {
            body.SetTrigger("idle");

            rb.velocity = new Vector2(0f, 0f);
        }

    }

    //for hiding arms while swipe animation plays
    private IEnumerator HideArms()
    {
        armSpriteRenderer.color = Color.clear;
        yield return new WaitForSeconds(6 / 24f); //swipe animation lasts 6 frames
        armSpriteRenderer.color = Color.white;
    }

    public void LockCharacterControls()
    {
        movementHalted = true;
    }

    public void UnlockCharacterControls()
    {
        movementHalted = false;
    }

    public void FaceLeft()
    {
        body.SetBool("faceLeft", true);
        body.SetBool("faceRight", false);
        arm.SetBool("faceLeft", true);
        arm.SetBool("faceRight", false);
        swipe.SetBool("faceLeft", true);
        swipe.SetBool("faceRight", false);

        leftHitBoxObject.SetActive(true);
        rightHitBoxObject.SetActive(false);
    }

    public void FaceRight()
    {
        body.SetBool("faceLeft", false);
        body.SetBool("faceRight", true);
        arm.SetBool("faceLeft", false);
        arm.SetBool("faceRight", true);
        swipe.SetBool("faceLeft", false);
        swipe.SetBool("faceRight", true);

        leftHitBoxObject.SetActive(false);
        rightHitBoxObject.SetActive(true);
    }

    //check for overlapping hitboxes
    #region
    public bool OverlapsLeftHitBox(Vector2 point)
    {
        return leftHitBoxObject.activeSelf && leftHitBox.OverlapPoint(point);
    }

    public bool OverlapsLeftHitBox(Collider2D collider)
    {
        return leftHitBoxObject.activeSelf && leftHitBox.IsTouching(collider);
    }

    public bool OverlapsRightHitBox(Vector2 point)
    {
        return rightHitBoxObject.activeSelf && rightHitBox.OverlapPoint(point);
    }

    public bool OverlapsRightHitBox(Collider2D collider)
    {
        return rightHitBoxObject.activeSelf && rightHitBox.IsTouching(collider);
    }

    public bool OverlapsHitBox(Vector2 point)
    {
        return hitBox.OverlapPoint(point);
    }

    public bool OverlapsHitBox(Collider2D collider)
    {
        return hitBox.IsTouching(collider);
    }
    #endregion
}
