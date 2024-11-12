using System.Collections;
using UnityEngine;

public class TalkingLights : MonoBehaviour
{
    public float syllableInterval = 0.3f; // Time interval for each syllable
    public int mouthPatternCount = 4; // Adjustable number of mouth patterns to use (up to 4)
    private MaterialToggle[] lights; // Array to hold 6 MaterialToggle components
    private Coroutine talkingRoutine;

    private readonly string sampleText = "Hello, how are you today?";

    // Define four mouth patterns for lights
    private readonly int[][] mouthPatterns = new int[][]
    {
        new int[] { },              // Closed - no lights
        new int[] { 2, 3 },         // Small Open - middle two lights
        new int[] { 1, 2, 3, 4 },   // Medium Open - four lights in the middle
        new int[] { 0, 1, 2, 3, 4, 5 } // Wide Open - all lights on
    };

    void Start()
    {
        // Populate the lights array with MaterialToggle components from child objects
        lights = GetComponentsInChildren<MaterialToggle>();
        
        if (lights.Length != 6)
        {
            Debug.LogWarning("TalkingLights script requires exactly 6 MaterialToggle components.");
        }
        
        StartTalking(sampleText); // Start talking animation with sample text
    }

    public void StartTalking(string text)
    {
        // Stop any ongoing talking sequence
        if (talkingRoutine != null)
        {
            StopCoroutine(talkingRoutine);
        }
        talkingRoutine = StartCoroutine(TalkingSequence(text));
    }

    private IEnumerator TalkingSequence(string text)
    {
        // Break the text into syllables
        string[] syllables = BreakTextIntoSyllables(text);

        foreach (string syllable in syllables)
        {
            // Choose a random mouth pattern from the available set
            int patternIndex1 = Random.Range(0, mouthPatternCount);
            int patternIndex2 = Random.Range(0, mouthPatternCount);

            // Apply the first mouth shape
            int[] mouthShape1 = mouthPatterns[patternIndex1];
            ApplyMouthShape(mouthShape1);
            Debug.Log($"Syllable: {syllable}, Mouth Shape 1: [{string.Join(", ", mouthShape1)}]");
            yield return new WaitForSeconds(syllableInterval / 2); // Half of the syllable interval

            // Apply the second mouth shape
            int[] mouthShape2 = mouthPatterns[patternIndex2];
            ApplyMouthShape(mouthShape2);
            Debug.Log($"Syllable: {syllable}, Mouth Shape 2: [{string.Join(", ", mouthShape2)}]");
            yield return new WaitForSeconds(syllableInterval / 2); // Second half of the syllable interval

            // Turn off all lights briefly between syllables
            ToggleAllLights(false);
            Debug.Log("Intermediate: All lights turned off after syllable.");
        }

        // Ensure all lights are off after the text sequence completes, with a slight delay
        yield return new WaitForSeconds(0.1f);
        ToggleAllLights(false);
        Debug.Log("End of text sequence - All lights turned off.");
    }

    private string[] BreakTextIntoSyllables(string text)
    {
        // Basic syllable splitting example for demonstration (splits by individual words for simplicity)
        return text.Split(new char[] { ' ', '-', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    private void ApplyMouthShape(int[] mouthShape)
    {
        // Turn off all lights first
        ToggleAllLights(false);

        // Turn on only the lights in the specified mouth shape pattern
        foreach (int index in mouthShape)
        {
            if (index >= 0 && index < lights.Length && lights[index] != null)
            {
                lights[index].SetToggleState(true);
            }
        }
    }

    private void ToggleAllLights(bool isOn)
    {
        foreach (var light in lights)
        {
            if (light != null)
            {
                light.SetToggleState(isOn);
            }
        }
    }
}
