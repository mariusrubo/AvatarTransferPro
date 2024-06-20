using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Not much to see here, just code rotating an object (in this project: a cube). This is done to visualize that the framerate remains relatively stable throughout
 * the application of novel character data thanks to async methods which avoid blocking the main thread.
 * */
public class RotateObject : MonoBehaviour
{
    float rotationSpeed = 180f; // in degrees per second

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
