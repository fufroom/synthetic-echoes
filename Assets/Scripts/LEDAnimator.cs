using System.IO.Ports;
using UnityEngine;

public class LEDAnimator : MonoBehaviour
{
    public string portName = "/dev/ttyUSB0";
    public int baudRate = 9600;
    private SerialPort serialPort;
    private bool isConnected = false;

    void Start()
    {
        TryInitializeSerialPort();
    }

    void TryInitializeSerialPort()
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
          // Debug.Log("LED Animator connected to serial port.");
        }
        catch (System.Exception ex)
        {
            isConnected = false;
            Debug.LogError($"Failed to open serial port: {ex.Message}");
        }
    }

    public void SendLEDCommand(string command)
    {
        if (isConnected && serialPort != null && serialPort.IsOpen)
        {
            if (int.TryParse(command, out int commandValue))
            {
                serialPort.WriteLine(commandValue.ToString());
                Debug.Log($"Sent LED command: {commandValue}");
            }
            else
            {
                Debug.LogError($"Invalid command format: {command}");
            }
        }
        else
        {
            Debug.LogError("Serial port is not connected.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SendLEDCommand("1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SendLEDCommand("2");
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
