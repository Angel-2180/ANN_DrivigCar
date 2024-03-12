using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MoveSpeed = 5.0f;

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input.Normalize();

        transform.position += input * MoveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale += 1;
            Time.fixedDeltaTime = Time.fixedUnscaledDeltaTime * Time.timeScale;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Time.timeScale -= 1;
            if (Time.timeScale < 1.0f)
                Time.timeScale = 1.0f;
            Time.fixedDeltaTime = Time.fixedUnscaledDeltaTime * Time.timeScale;
        }
    }
}
