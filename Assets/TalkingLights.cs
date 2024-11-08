using System.Collections;
using UnityEngine;

public class TalkingLights : MonoBehaviour
{
    public float syllableInterval = 0.3f; // Time interval for each syllable
    private MaterialToggle[] lights; // Array to hold 6 MaterialToggle components
    private Coroutine talkingRoutine;

    private readonly string sampleText = "Hello, how are you today?";

    // Define two sequences for each mouth shape using light indices
    private readonly int[][] mouthPatterns1 = new int[][]
    {
        new int[] { },              // Closed - no lights
        new int[] { 2, 3 },         // Small Open - middle two lights
        new int[] { 1, 2, 3, 4 },   // Medium Open - four lights in the middle
        new int[] { 0, 1, 2, 3, 4, 5 }, // Wide Open - all lights on
        new int[] { 1, 4 },         // Partially Open - two lights in wider position
        new int[] { 0, 5 }          // Outer Lights - corners only
    };

    private readonly int[][] mouthPatterns2 = new int[][]
    {
        new int[] { },              // Closed - no lights
        new int[] { 1, 4 },         // Small Open alternate - wider two lights
        new int[] { 0, 5 },         // Medium Open alternate - outer lights
        new int[] { 2, 3 },         // Wide Open alternate - middle two lights
        new int[] { 0, 5 },         // Partially Open alternate - outer corners
        new int[] { 1, 2, 3, 4 }    // Outer Lights alternate - middle four
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
        
        int patternIndex = 0; // Start with the first pattern

        foreach (string syllable in syllables)
        {
            // First flash of the mouth shape
            int[] mouthShape1 = mouthPatterns1[patternIndex % mouthPatterns1.Length];
            ApplyMouthShape(mouthShape1);
            Debug.Log($"Syllable: {syllable}, Mouth Shape 1: [{string.Join(", ", mouthShape1)}]");
            yield return new WaitForSeconds(syllableInterval / 2); // Half of the syllable interval

            // Second flash of the mouth shape
            int[] mouthShape2 = mouthPatterns2[patternIndex % mouthPatterns2.Length];
            ApplyMouthShape(mouthShape2);
            Debug.Log($"Syllable: {syllable}, Mouth Shape 2: [{string.Join(", ", mouthShape2)}]");
            yield return new WaitForSeconds(syllableInterval / 2); // Second half of the syllable interval
            
            // Turn off all lights briefly between syllables
            ToggleAllLights(false);
            Debug.Log("Intermediate: All lights turned off after syllable.");

            // Move to the next pattern, cycling back to the start if needed
            patternIndex++;
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
