using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour
{
    private Button skipButton;
    private bool previousButton1State = false;
    private bool previousButton2State = false;

    void Awake()
    {
        skipButton = GetComponent<Button>();
        if (skipButton == null)
        {
            Debug.LogError("SkipButton script requires a Button component on the same GameObject.");
        }
    }

    void Update()
    {
        if (PalmScanner.button1Pressed && !previousButton1State)
        {
            previousButton1State = true;
            PressButton();
        }
        else if (!PalmScanner.button1Pressed)
        {
            previousButton1State = false;
        }

        if (PalmScanner.button2Pressed && !previousButton2State)
        {
            previousButton2State = true;
            PressButton();
        }
        else if (!PalmScanner.button2Pressed)
        {
            previousButton2State = false;
        }
    }

    void PressButton()
    {
        if (skipButton != null)
        {
            skipButton.onClick.Invoke();
            //Debug.Log("SkipButton pressed via keyboard input.");
        }
    }
}
