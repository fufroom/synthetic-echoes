using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"SkipButton on {gameObject.name} is missing a Button component.");
        }
    }

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(() => Debug.Log($"Button {gameObject.name} clicked."));
        }
    }

    public void TriggerButton()
    {
        if (button != null && button.interactable)
        {
            Debug.Log($"Button {gameObject.name} triggered via script.");
            button.onClick.Invoke();
        }
        else
        {
            Debug.LogWarning($"Button {gameObject.name} is not interactable.");
        }
    }

    public void HandlePalmScannerInput(string receivedData)
    {
        Debug.Log($"Palm scanner input received: {receivedData} for button {gameObject.name}");
        TriggerButton();
    }
}
