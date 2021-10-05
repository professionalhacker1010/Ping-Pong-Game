using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDevDesktopScreen : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float minimizedCameraSize;
    [SerializeField] private Vector3 minimizedCameraPosition;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
        cam.orthographicSize = minimizedCameraSize;
        cam.transform.position = minimizedCameraPosition;
    }
    // Update is called once per frame
    private void OnMouseDown()
    {
        cam.orthographicSize = 5.375873f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        print("enable");
        cam = FindObjectOfType<Camera>();
        cam.orthographicSize = minimizedCameraSize;
        cam.transform.position = minimizedCameraPosition;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        cam = FindObjectOfType<Camera>();
        cam.orthographicSize = 5.375873f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
