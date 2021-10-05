using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCodes
{
    /*[SerializeField] List<KeyCode> hitKeys;
    [SerializeField] List<KeyCode> interactKeys;
    [SerializeField] List<KeyCode> leftKeys;
    [SerializeField] List<KeyCode> rightKeys;
    [SerializeField] List<KeyCode> pauseKeys;*/

    public static bool Interact()
    {
        return Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }

    public static bool InteractGetUp()
    {
        return Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift);
    }

    public static bool Hit()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
    }
    public static bool HitGetUp ()
    {
        return Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return);
    }
    
    public static bool HitHoldDown()
    {
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return);
    }

    public static bool Left()
    {
        return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
    }

    public static bool Right()
    {
        return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
    }

    public static bool Pause()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public static bool PauseGetUp()
    {
        return Input.GetKeyUp(KeyCode.Escape);
    }
}
