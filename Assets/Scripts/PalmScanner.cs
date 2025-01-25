using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

public class PalmScanner : MonoBehaviour
{
    public Image connectionIcon;
    private SerialPort serialPort;
    public string portName = "/dev/ttyUSB0";  // Adjust for Linux
    public int baudRate = 9600;
    private bool isConnected = false;
    public static bool button1Pressed = false;
    public static bool button2Pressed = false;
    private float button1ResetTime;
    private float button2ResetTime;
    private const float resetDelay = 1f;

    void Start()
    {
        TryInitializeScanner();
    }

    void TryInitializeScanner()
    {
        try
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }

            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 100;
            isConnected = serialPort.IsOpen;
        }
        catch (System.Exception ex)
        {
            isConnected = false;
            serialPort = null;
            UnityEngine.Debug.LogError($"Failed to open serial port: {ex.Message}");
        }
        UpdateIconColor();
    }

    void Update()
    {
        if (serialPort == null || !isConnected)
        {
            return;
        }

        try
        {
            if (serialPort.BytesToRead > 0)
            {
                string receivedData = serialPort.ReadLine().Trim();
                UnityEngine.Debug.Log($"Palm Scanner received: {receivedData}");

                if (receivedData == "1")
                {
                    button1Pressed = true;
                    button1ResetTime = Time.time + resetDelay;
                }
                else if (receivedData == "2")
                {
                    button2Pressed = true;
                    button2ResetTime = Time.time + resetDelay;
                }
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"Error reading from serial port: {ex.Message}");
        }

        if (button1Pressed && Time.time >= button1ResetTime)
        {
            button1Pressed = false;
        }

        if (button2Pressed && Time.time >= button2ResetTime)
        {
            button2Pressed = false;
        }
    }

    void UpdateIconColor()
    {
        if (connectionIcon != null)
        {
            connectionIcon.color = isConnected ? Color.yellow : Color.black;
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
