using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using System.Collections.Generic;

public class InkPlayer : MonoBehaviour
{
    public Text storyText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    private Story inkStory;

    void Start()
    {
        TextAsset inkJSON = Resources.Load<TextAsset>("story");
        inkStory = new Story(inkJSON.text);
        RefreshView();
    }

    void RefreshView()
    {
        storyText.text = inkStory.Continue();

        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        if (inkStory.currentChoices.Count > 0)
        {
            foreach (var choice in inkStory.currentChoices)
            {
                GameObject button = Instantiate(choiceButtonPrefab, choiceContainer);
                button.GetComponentInChildren<Text>().text = choice.text;
                button.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice.index));
            }
        }
    }

    void OnChoiceSelected(int choiceIndex)
    {
        inkStory.ChooseChoiceIndex(choiceIndex);
        RefreshView();
    }
}
