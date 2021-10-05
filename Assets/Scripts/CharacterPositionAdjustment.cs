using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPositionAdjustment : MonoBehaviour
{
    private float horizontalInput = 0;
    [SerializeField] CharacterControls characterControls;
    // Update is called once per frame
    void Update()
    {
        characterControls.MoveCharacter(horizontalInput);
    }

    public void InitializeAdjustment(float horizontal)
    {
        horizontalInput = horizontal;
    }
}
