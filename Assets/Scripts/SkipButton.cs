using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (button != null && button.interactable)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Keyboard input detected: 1");
                TriggerButton();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Keyboard input detected: 2");
                TriggerButton();
            }
        }
    }

    public void TriggerButton()
    {
        if (button != null && button.interactable)
        {
            Debug.Log($"Button {button.name} triggered.");
            button.onClick.Invoke();
        }
    }

    public void HandlePalmScannerInput(string receivedData)
    {
        if (receivedData == "1" || receivedData == "2")
        {
            Debug.Log($"Palm scanner input received: {receivedData}");
            TriggerButton();
        }
    }
}
