using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;

public class InkPlayer : MonoBehaviour
{
    public TMP_Text storyText; // TextMeshPro component for story content
    public Transform choiceContainer; // Parent for choice buttons
    public GameObject choiceButtonPrefab; // Prefab for a choice button

    private Story inkStory;

    void Start()
    {
        // Load Ink JSON
        TextAsset inkJSON = Resources.Load<TextAsset>("story");
        if (inkJSON == null)
        {
            Debug.LogError("story.json file not found in Resources folder!");
            return;
        }

        inkStory = new Story(inkJSON.text);
        RefreshView();
    }

    void RefreshView()
    {
        // Clear previous UI
        storyText.text = "";
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        // Add story text
        while (inkStory.canContinue)
        {
            storyText.text += inkStory.Continue();
        }

        // Add choices
        if (inkStory.currentChoices.Count > 0)
        {
            foreach (Choice choice in inkStory.currentChoices)
            {
                GameObject button = Instantiate(choiceButtonPrefab, choiceContainer);
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                buttonText.text = choice.text;

                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    OnChoiceSelected(choice.index);
                });
            }
        }
        else
        {
            // Add a "Continue" button if no choices
            GameObject continueButton = Instantiate(choiceButtonPrefab, choiceContainer);
            TMP_Text buttonText = continueButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = "Continue";

            continueButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                RefreshView();
            });
        }
    }

    void OnChoiceSelected(int choiceIndex)
    {
        inkStory.ChooseChoiceIndex(choiceIndex);
        RefreshView();
    }
}
