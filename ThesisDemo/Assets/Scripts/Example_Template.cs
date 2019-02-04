// This is a blank template for use with Arduino_AllInput

using UnityEngine;
using System.Collections;

public class Example_Template : MonoBehaviour
{
    // A variable for the static  Arduino_AllInputs GameObject in the Hierarchy
    private Arduino_AllInputs arduino;

    void Start()
    {
        // The Arduino_AllInputs prefab is a static instance. It must be in your Hierarchy
        arduino = Arduino_AllInputs.instance;
    }

    void Update()
    {
        // Check for Arduino input. Don't continue if Arduino is not ready
        if (!arduino.Ready())
        {
            return;
        }

        // Do something with the Arduino Input here

    }
}