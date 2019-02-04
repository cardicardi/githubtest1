// Example expects Analog on pin A0 and Digital input on pin D3

using UnityEngine;
using System.Collections;

public class Example_AllInputs : MonoBehaviour
{
    private Arduino_AllInputs arduino;

    void Start()
    {
        // The Arduino_AllInputs prefab must be in your scene
        arduino = Arduino_AllInputs.instance;
    }

    void Update()
    {
        // Check for Arduino input. Don't continue if Arduino is not ready.
        if (!arduino.Ready())
        {
            return;
        }

        // Basic analog and digital input
        Debug.Log("A0: " + arduino.GetAnalogInput(0) + " D3: " + arduino.GetDigitalInput(3));

        // Button-like digital input
        if (arduino.GetButtonDown(3))
        {
            Debug.Log("Button 3 pressed");
        }

    }
}
