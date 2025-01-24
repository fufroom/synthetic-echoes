using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PalmScanner : MonoBehaviour
{
    public Image connectionIcon;
    private SerialPort serialPort;
    public string portName = "/dev/ttyUSB0";
    public int baudRate = 9600;
    private DialogueManager dialogueManager;
    private bool isConnected = false;
    private bool scannerEnabled = true;

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
            Debug.Log("Palm Scanner connected.");
            UpdateIconColor();
            StartCoroutine(FindDialogueManager());
        }
        catch (System.Exception ex)
        {
            isConnected = false;
            Debug.Log($"Failed to open serial port: {ex.Message}");
            UpdateIconColor();
        }
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

    private IEnumerator FindDialogueManager()
    {
        while (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {
        if (scannerEnabled && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    string receivedData = serialPort.ReadLine().Trim();
                    Debug.Log($"Palm Scanner received: {receivedData}");

                    if (!string.IsNullOrEmpty(receivedData) && (receivedData == "1" || receivedData == "2"))
                    {
                        ProcessButtonPress(receivedData);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log($"Error reading from serial port: {ex.Message}");
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            scannerEnabled = !scannerEnabled;
            if (scannerEnabled)
            {
                Debug.Log("Palm Scanner re-enabled.");
                TryInitializeScanner();
            }
            else
            {
                Debug.Log("Palm Scanner disabled.");
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
        }
    }

    void ProcessButtonPress(string receivedData)
    {
        if (dialogueManager != null && dialogueManager.gameObject.activeInHierarchy)
        {
            if (dialogueManager.dialogueUI.choiceContainer.gameObject.activeSelf)
            {
                Debug.Log($"Sending choice selection to DialogueManager: {receivedData}");
                dialogueManager.HandleChoiceSelection(receivedData);
                return;
            }
        }

        SkipButton[] skipButtons = FindObjectsOfType<SkipButton>(true);
        foreach (var skipButton in skipButtons)
        {
            if (skipButton.gameObject.activeInHierarchy)
            {
                Debug.Log($"Palm scanner triggering SkipButton with input: {receivedData}");
                skipButton.HandlePalmScannerInput(receivedData);
                return;
            }
        }

        Debug.LogWarning("No valid target found for palm scanner input.");
    }

    public void TriggerLEDAnimation(int effectType)
    {
        if (serialPort != null && serialPort.IsOpen && scannerEnabled)
        {
            serialPort.WriteLine(effectType.ToString());
            Debug.Log($"Sent LED animation command: {effectType}");
        }
        else
        {
            Debug.Log("Cannot send LED animation. Serial port is not connected.");
        }
    }

    void UpdateIconColor()
    {
        if (connectionIcon != null)
        {
            connectionIcon.color = isConnected ? Color.white : Color.black;
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
