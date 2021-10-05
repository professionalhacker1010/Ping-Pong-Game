using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float distance; //0 means it follows the camera as if it were in the foreground (no edit to transform), 1 means that it stays completely still relative to camera
    [SerializeField] private float startParallax;
    //public float distance;
    private Camera cam;
    private float prevCamX;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
        prevCamX = cam.transform.position.x;
    }

    private void Update()
    {
        float camX = cam.transform.position.x;
        if (camX >= startParallax)
        {
            float x = distance * (camX - prevCamX);
            transform.position = new Vector3(transform.position.x + x, transform.position.y);
        }
        prevCamX = camX;
    }
}
