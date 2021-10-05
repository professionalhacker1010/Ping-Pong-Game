using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Camera cam;
    [SerializeField] float strengthZeroRadius, strengthOneRadius;
    [SerializeField] float shakeIntervalTime;
    private float startX, startY;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        startX = transform.position.x;
        startY = transform.position.y;
    }

    public void ShakeCamera(int strength)
    {
        StartCoroutine(ShakeCameraHelper(strength));
    }

    private IEnumerator ShakeCameraHelper(int strength)
    {
        float radius = 0;
        if (strength == 0)
        {
            radius = strengthZeroRadius;
        }
        else if (strength == 1)
        {
            radius = strengthOneRadius;
        }

        float Z = -10f;
        //shake in a start pattern
        //left
        cam.transform.position = new Vector3(-1 * radius + startX, startY, Z);
        yield return new WaitForSeconds(shakeIntervalTime);

        //right
        cam.transform.position = new Vector3(radius + startX, startY, Z);
        yield return new WaitForSeconds(shakeIntervalTime);

        //bottom left
        cam.transform.position = new Vector3(-1 * radius + startX, -1 * radius + startY, Z);
        yield return new WaitForSeconds(shakeIntervalTime);

        //up
        cam.transform.position = new Vector3(startX, radius + startY, Z);
        yield return new WaitForSeconds(shakeIntervalTime);

        //bottom right (half radius)
        cam.transform.position = new Vector3(radius/2 + startX, -1 * radius/2 + startY, Z);
        yield return new WaitForSeconds(shakeIntervalTime);

        cam.transform.position = new Vector3(startX, startY, Z);
    }
}
