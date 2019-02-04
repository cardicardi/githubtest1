// Quick & Dirty Cross Platform Arduino -> Unity: All Inputs
// YMMV
// Sam Sheffield (hello@samsheffield.com)

using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

public class Arduino_AllInputs : MonoBehaviour
{
    public static SerialPort serialPort;
    [Tooltip("Windows: Ports higher than 9 need \\\\.\\\\ in front")]
    public string portName = "COM3";
    public int baudRate = 9600;
    [Tooltip("Poll Arduino with Coroutine (set interval) or during Update (fastest, but may affect performance)")]
    public Coroutine pollingMethod;
    [Tooltip("These are characters sent & received from Arduino")]
    public Communication controlCharacters;
    public int[] analogInput = new int[6];
    public bool[] digitalInput = new bool[14];
    private bool[] lastDigitalInput = new bool[14];
    public static Arduino_AllInputs instance;

    private int totalInputs = 20;
    private int connectData;
    private int[] inputData;
    private bool connected;


    // PUBLIC METHODS

    // Get specific Analog Input (by pin number)
    public int GetAnalogInput(int inputNumber)
    {
        return analogInput[inputNumber];
    }

    // Get specific Digital Input (by pin number)
    public bool GetDigitalInput(int inputNumber)
    {
        return digitalInput[inputNumber];
    }

    // Check if a digital input is pressed
    public bool GetButtonDown(int inputNumber)
    {
        bool comparedInput = (digitalInput[inputNumber] && !lastDigitalInput[inputNumber]);
        lastDigitalInput[inputNumber] = digitalInput[inputNumber];

        return comparedInput;
    }

    // Has Arduino established contact?
    public bool Ready()
    {
        return connected;
    }

    // Connect to the Arduino 
    public void Connect(string portName)
    {
        serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        if (serialPort != null)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                Debug.Log("Port already open. Closing...");
            }
            else
            {
                serialPort.Open();
                serialPort.ReadTimeout = 12;
                Debug.Log("Connected to Serial port (" + portName + ").");
            }
        }
        else
        {
            if (serialPort.IsOpen)
            {
                Debug.Log("Port is already open");
            }
            else
            {
                Debug.Log("Port == null");
            }
        }
    }

    // PRIVATE METHODS

    // Set up Arduino
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GetPortNames();
            Connect(portName);

            // If using the coroutine for Arduino polling
            if (pollingMethod.usingCoroutine)
                StartArduino();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        inputData = new int[totalInputs];
    }

    private void Update()
    {
        if (!pollingMethod.usingCoroutine)
            Read();
    }

    // List port numbers for connected Arduinos
    private void GetPortNames()
    {
        int p = (int)System.Environment.OSVersion.Platform;
        List<string> serialPorts = new List<string>();

        if (p == 4 || p == 128 || p == 6)
        {
            string[] ttys = Directory.GetFiles("/dev/", "tty.*");
            foreach (string dev in ttys)
            {
                if (dev.StartsWith("/dev/tty.*") || dev.StartsWith("/dev/cu.*"))
                    serialPorts.Add(dev);

                Debug.Log("Available Arduino port: " + System.String.Format(dev));
            }
        }
        else
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                Debug.Log("Available Arduino port: " + port.ToString());
            }
        }
    }

    // Close port and send signal to reset the Arduino 
    private void OnApplicationQuit()
    {
        if (serialPort != null)
        {
            serialPort.Write(controlCharacters.endCharacter.ToString());
            serialPort.Close();
            Debug.Log("Port closed");
        }
    }

    // Read bytes from Arduino
    private void Read()
    {
        try
        {
            // After receiving a control char from the Arduino, begin communicating back and forth
            if (!Ready())
            {
                connectData = serialPort.ReadByte();

                if (connectData == controlCharacters.receiveCharacter)
                {
                    serialPort.DiscardInBuffer();
                    connected = true;
                    serialPort.Write(controlCharacters.sendCharacter.ToString());
                }
            }
            else
            {

                for (int i = 0; i < totalInputs; i++)
                {
                    inputData[i] = serialPort.ReadByte();

                    if (i < analogInput.Length)
                        analogInput[i] = inputData[i];
                    else
                        digitalInput[i - analogInput.Length] = inputData[i] < 1;
                }
                serialPort.Write(controlCharacters.sendCharacter.ToString());
            }
        }
        catch (System.Exception e)
        {
            if (e.GetType() == typeof(System.TimeoutException))
                return;
            else
                throw;
        }
    }

    // Start polling coroutine
    private void StartArduino()
    {
        if (serialPort.IsOpen && serialPort != null)
        {
            StartCoroutine(PollArduino());
        }
    }

    // Get input from Arduino
    private IEnumerator PollArduino()
    {
        while (true)
        {
            yield return new WaitForSeconds(pollingMethod.pollingInterval);
            Read();
        }
    }

    [System.Serializable]
    public class Communication
    {
        public char sendCharacter = 'A';
        public char receiveCharacter = 'A';
        public char endCharacter = 'Z';
    }

    [System.Serializable]
    public class Coroutine
    {
        public bool usingCoroutine = true;
        public float pollingInterval = .033f;
    }
}
