using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text speakerText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    public DialogueController dialogueController;

    private bool previousButton1State = false;
    private bool previousButton2State = false;

    private bool ChoicesEnabled = false;

 void Update()
{
    if ( choiceContainer.gameObject.activeInHierarchy)
    {
        if (PalmScanner.button1Pressed && !previousButton1State)
        {
            previousButton1State = true;
            HandleChoiceSelection("1");
        }
        else if (!PalmScanner.button1Pressed)
        {
            previousButton1State = false;
        }

        if (PalmScanner.button2Pressed && !previousButton2State)
        {
            previousButton2State = true;
            HandleChoiceSelection("2");
        }
        else if (!PalmScanner.button2Pressed)
        {
            previousButton2State = false;
        }
    }
}


    public void RefreshView(DialogueNode node, DialogueManager manager)
    {
        choiceContainer.gameObject.SetActive(false);
        Debug.Log($"Refreshing view for node ID: {node.id}");
        Debug.Log($"Setting dialogue title: {node.speaker}, body: {node.GetBodyText()}");

        dialogueController.SetDialogue(node.speaker ?? "", node.GetBodyText() ?? "");

        ClearChoices();
        
        Debug.Log("Cleared previous choices.");

        if (node.text_to_speech)
        {
            Debug.Log($"Text-to-speech enabled for node ID: {node.id}, attempting to play voice.");
            manager.dialogueAudio.PlayVoice(node.id, () => EnableChoices(node.choices, manager));
        }
        else
        {
            Debug.Log("Text-to-speech disabled, enabling choices immediately.");
            EnableChoices(node.choices, manager);
        }
    }

void Start(){
    choiceContainer.gameObject.SetActive(false);
}
    void ClearChoices()
    {
        choiceContainer.gameObject.SetActive(false);
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Choice container cleared.");
    }

    void EnableChoices(List<DialogueNode.Choice> choices, DialogueManager manager)
    {

        foreach (var choice in choices)
        {
            if (!string.IsNullOrEmpty(choice.text) && !string.IsNullOrEmpty(choice.button))
            {
                Debug.Log($"Adding choice: {choice.text} with button: {choice.button}");
                GameObject button = Instantiate(choiceButtonPrefab, choiceContainer);
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                buttonText.text = $"[{choice.button}] {choice.text}";
                button.GetComponent<Button>().onClick.AddListener(() => manager.HandleChoiceSelection(choice.button));
            }
            else
            {
                Debug.LogWarning("Encountered a choice with missing text or button, skipping.");
            }
        }
          choiceContainer.gameObject.SetActive(true);
    }

    void HandleChoiceSelection(string buttonPressed)
    {

        Debug.Log("==============================================/n Tried to press button for " + buttonPressed);

        foreach (Transform child in choiceContainer)
        {
            Button button = child.GetComponent<Button>();
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

Debug.Log("buttonText.text.Contains($\"[{buttonPressed}]\")) " + buttonText.text.Contains($"[{buttonPressed}]")) ;
            if (buttonText.text.Contains($"[{buttonPressed}]"))
            {
                Debug.Log($"Simulating click for button: {buttonPressed}");
                button.onClick.Invoke();
                choiceContainer.gameObject.SetActive(false);
                return;
            }
        }
    }
}
