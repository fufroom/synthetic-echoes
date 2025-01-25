using System.IO.Ports;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;   // Added for IEnumerator
using UnityEngine.UI;       // Added for Image

public class PalmScanner : MonoBehaviour
{
    public Image connectionIcon;
    private SerialPort serialPort;
    public string portName = "COM3";  // Change to match your port (e.g., "/dev/ttyUSB0" on Linux/Mac)
    public int baudRate = 9600;
    private bool isConnected = false;

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    private const byte VK_1 = 0x31;  // Virtual keycode for '1'
    private const byte VK_2 = 0x32;  // Virtual keycode for '2'
    private const uint KEYEVENTF_KEYDOWN = 0x0000;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    void Start()
    {
        TryInitializeScanner();
        StartCoroutine(CheckConnectionStatus());
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
            Debug.Log("Palm Scanner connected successfully.");
        }
        catch (System.Exception ex)
        {
            isConnected = false;
            serialPort = null;
            Debug.LogError($"Failed to open serial port: {ex.Message}");
        }
        UpdateIconColor();
    }

    private IEnumerator CheckConnectionStatus()
    {
        while (true)
        {
            isConnected = serialPort != null && serialPort.IsOpen;
            UpdateIconColor();
            yield return new WaitForSeconds(2f);
        }
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
                Debug.Log($"Palm Scanner received: {receivedData}");

                if (receivedData == "1")
                {
                    SimulateKeyPress(VK_1);
                }
                else if (receivedData == "2")
                {
                    SimulateKeyPress(VK_2);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error reading from serial port: {ex.Message}");
        }
    }

    void SimulateKeyPress(byte keyCode)
    {
        Debug.Log($"Simulating key press: {keyCode}");
        keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(keyCode, 0, KEYEVENTF_KEYUP, 0);
    }

    void UpdateIconColor()
    {
        if (connectionIcon != null)
        {
            connectionIcon.color = isConnected ? Color.green : Color.red;
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
