using UnityEngine;
using TMPro;

public class DebugConsole : MonoBehaviour
{

    public GameObject debugConsoleBG;
    public TextMeshProUGUI debugText;
    private bool isVisible = false;
    private string logContent = "";

    void Awake()
    {
        if (debugText == null)
        {
            Debug.LogError("DebugConsole: TextMeshProUGUI reference not set!");
            return;
        }
        
        debugConsoleBG.gameObject.SetActive(false);
        debugText.gameObject.SetActive(false);
        Application.logMessageReceived += HandleLog;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            isVisible = !isVisible;
            debugText.gameObject.SetActive(isVisible);
            debugConsoleBG.gameObject.SetActive(isVisible);
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logContent += logString + "\n";
        debugText.text = logContent;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
