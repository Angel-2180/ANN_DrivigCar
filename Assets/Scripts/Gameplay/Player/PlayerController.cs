using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    Car playerCar;

    void Awake()
    {
        playerCar = GetComponent<Car>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (playerCar == null)
            return;

        CarInput inputs = new CarInput()
        {
            Thrust = Input.GetAxis("Vertical"),
            Steering = Input.GetAxis("Horizontal")
        };

        playerCar.Move(inputs);
    }
}
