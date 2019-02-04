using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example_Mic : MonoBehaviour
{
    public int threshold = 127;
    private Arduino_AllInputs arduino;

    // Use this for initialization
    void Start()
    {
        arduino = Arduino_AllInputs.instance;
    }

    void Update()
    {
        // Check analog input to see if it has exceeded a threshold
        if (arduino.GetAnalogInput(0) >= threshold)
        {
            Debug.Log("Above threshold");
        }

        // Check analog input to see if it is in a particular range
        if(arduino.GetAnalogInput(0) >= 30 && arduino.GetAnalogInput(0) <= 40)
        {
            Debug.Log("In range");
        }
    }
}
