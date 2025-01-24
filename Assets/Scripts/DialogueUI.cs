using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text speakerText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    public DialogueController dialogueController;

    public void RefreshView(DialogueNode node, DialogueManager manager)
    {
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

    void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Choice container cleared.");
    }

    void EnableChoices(List<DialogueNode.Choice> choices, DialogueManager manager)
    {
        Debug.Log($"Enabling {choices.Count} choices.");
        
        foreach (var choice in choices)
        {
            if (!string.IsNullOrEmpty(choice.text) && !string.IsNullOrEmpty(choice.button))
            {
                Debug.Log($"Adding choice: {choice.text} with button: {choice.button}");
                GameObject button = Instantiate(choiceButtonPrefab, choiceContainer);
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                buttonText.text = $"[{choice.button}] {choice.text}";
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => manager.HandleChoiceSelection(choice.button));
            }
            else
            {
                Debug.LogWarning("Encountered a choice with missing text or button, skipping.");
            }
        }
    }
}
