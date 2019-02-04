// Example expects one Analog and one Digital input

using UnityEngine;
using System.Collections;

public class Example_ManualInputs : MonoBehaviour
{
    private Arduino_ManualInputs arduino;

    void Start()
    {
        // The Arduino_ManualInputs prefab must be in your scene
        arduino = Arduino_ManualInputs.instance;
    }

    void Update()
    {
        // Check for Arduino input. Don't continue if Arduino is not ready.
        if (!arduino.Ready())
        {
            return;
        }

        // Basic analog and digital input
        Debug.Log("Analog: " + arduino.GetAnalogInput(0) + " Digital: " + arduino.GetDigitalInput(0));

        // Button-like digital input
        if (arduino.GetButtonDown(0))
        {
            Debug.Log("Digital pressed");
        }

    }
}
