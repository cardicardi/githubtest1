// Example expects Analog on pin A0 and Digital input on pin D3

using UnityEngine;
using System.Collections;

public class Example_Sound : MonoBehaviour
{
    private Arduino_AllInputs arduino;
    private AudioSource aSource;
    public AudioClip clip;

    public int analogInput, digitalInput;
    public float mapFrom1, mapTo1, mapFrom2, mapTo2;

    void Start()
    {
        // The Arduino_AllInputs prefab must be in your scene
        arduino = Arduino_AllInputs.instance;
        aSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check for Arduino input. Don't continue if Arduino is not ready.
        if (!arduino.Ready())
        {
            return;
        }

        // Button-like digital input
        if (arduino.GetButtonDown(digitalInput))
        {
            aSource.pitch = Map(arduino.GetAnalogInput(analogInput), mapFrom1, mapTo1, mapFrom2, mapTo2);
            aSource.PlayOneShot(clip);
        }

    }

    // Output a range of numbers mapped to another range (Processing style!)
    // Example: Map(arduino.GetAnalogInput(analogInput), 0, 255, 0.25f,2) maps the range 0-255 to 0.25 - 2
    float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
