using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;

public class InkPlayer : MonoBehaviour
{
    public TMP_Text storyText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    private Story inkStory;

    void Start()
    {
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
    storyText.text = ""; // Clear existing story text

    // Clear all existing choice buttons
    foreach (Transform child in choiceContainer)
    {
        Destroy(child.gameObject);
    }

    // Display all story content available at this point
    while (inkStory.canContinue)
    {

        string nextStoryBit = inkStory.Continue().Trim() + "\n";
        Debug.Log(nextStoryBit);
        storyText.text = "";
        storyText.text += nextStoryBit;
    }

    // Handle choices
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
    else if (!inkStory.canContinue) // Add "Continue" only if no choices and no content
    {
        AddContinueButton();
    }
}

void AddContinueButton()
{
    GameObject continueButton = Instantiate(choiceButtonPrefab, choiceContainer);
    TMP_Text buttonText = continueButton.GetComponentInChildren<TMP_Text>();
    buttonText.text = "Continue";

    continueButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
    {
        RefreshView();
    });
}


    void OnChoiceSelected(int choiceIndex)
    {
        inkStory.ChooseChoiceIndex(choiceIndex);
        RefreshView();
    }
}
