using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class InkPlayer : MonoBehaviour
{
    public TMP_Text storyText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    public AudioSource audioSource;

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
            storyText.text += nextStoryBit;

            // Play voice for the current text
            PlayVoice(nextStoryBit);
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
        // Stop any currently playing audio
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        inkStory.ChooseChoiceIndex(choiceIndex);
        RefreshView();
    }

    void PlayVoice(string text)
{
    // Generate the deterministic filename for the audio file
    string filename = GenerateVoiceFilename(text);
    string resourcePath = $"sounds/voice/{filename.Replace(".wav", "")}";

    // Load the audio clip and play it
    AudioClip clip = Resources.Load<AudioClip>(resourcePath);
    if (clip != null)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
    else
    {
        Debug.LogWarning($"AudioClip not found for {resourcePath}");
    }
}

    string GenerateVoiceFilename(string text)
    {
        int randomValue = SeededRandom(text);
        return $"voice_{randomValue}.wav";
    }

    int SeededRandom(string text, int seed = 12345)
    {
        // Sanitize the text
        string sanitized = SanitizeText(text);

        // Generate seed value from sanitized text
        int seedValue = seed;
        foreach (char c in sanitized)
        {
            seedValue += c;
        }

        // LCG algorithm
        long randomNumber = (seedValue * 1103515245L + 12345) & 0x7FFFFFFF;
        return (int)(randomNumber % 100000000); // Limit to 8 digits
    }

    string SanitizeText(string text)
    {
        // Convert to lowercase and remove spaces/punctuation
        return Regex.Replace(text.ToLower(), @"[^a-zA-Z0-9]", "");
    }
}
