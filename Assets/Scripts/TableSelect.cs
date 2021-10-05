using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSelect : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private BoxCollider2D playerCollider;
    private BoxCollider2D boxCollider;
    private Vector3 newTransform;
    private bool indicatorPlaying = false, thisSelectable = true;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        newTransform.x = transform.position.x;
        newTransform.y = transform.position.y + 0.5f;
    }

    private void Update()
    {
        if (boxCollider.IsTouching(playerCollider) && TableSelectManager.Instance.selectable && thisSelectable)
        {
            if (!indicatorPlaying) StartCoroutine(TouchTableIndicator());

            if (KeyCodes.Interact() && level <= LevelManager.currOpponent) StartCoroutine(TableSelectManager.Instance.TransitionToGame(level));
        }
    }

    private IEnumerator TouchTableIndicator()
    {
        indicatorPlaying = true;
        transform.position = newTransform;
        newTransform.y -= 0.5f;
        yield return new WaitUntil(() => (!boxCollider.IsTouching(playerCollider)) || !TableSelectManager.Instance.selectable);
        indicatorPlaying = false;
        transform.position = newTransform;
        newTransform.y += 0.5f;
        //for now
    }
    
    public void LockThisTable()
    {
        thisSelectable = false;
    }

    public void UnlockThisTable()
    {
        thisSelectable = true;
    }
}
