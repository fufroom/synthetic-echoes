using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BootSequence : MonoBehaviour
{
    public TextMeshProUGUI bootTextBox; // Text box for displaying boot sequence
    public string bootTextFileName = "Boot_sequence";

    [Header("Typing Speed (characters per second; higher = faster)")]
    public float minTypingSpeedCharactersPerSecond = 300f;
    public float maxTypingSpeedCharactersPerSecond = 700f;

    [Header("Line Delay (seconds; higher = slower)")]
    public float minLineDelaySeconds = 0.3f;
    public float maxLineDelaySeconds = 1.0f;

    public int lineLimit = 15; // Max number of lines displayed

    [Header("On Boot Complete")]
    public float delayAfterLastLineSeconds = 1.5f;
    public UnityEvent onBootSequenceComplete;

    private List<string> bootText = new List<string>();
    private List<string> displayedLines = new List<string>();
    private int lineIndex = 0;
    private float typingSpeedCharactersPerSecond;
    private float typingAccumulator = 0f;
    private int currentChar = 0;
    private string currentLine = "";

    void Start()
    {
        LoadTextFromFile();
        typingSpeedCharactersPerSecond = Random.Range(minTypingSpeedCharactersPerSecond, maxTypingSpeedCharactersPerSecond);

        if (bootText.Count > 0)
        {
            StartCoroutine(DisplayNextLine());
        }
        else
        {
            Console.Log("No text found in the boot sequence file!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Console.Log("User skip activated");
            onBootSequenceComplete?.Invoke();
        }
    }

    private void LoadTextFromFile()
    {
        TextAsset file = Resources.Load<TextAsset>(bootTextFileName);
        if (file != null)
        {
            bootText = new List<string>(file.text.Split('\n'));
            //Console.Log("Boot sequence text loaded successfully.");
            //Console.Log(string.Join(", ", bootText));
        }
        else
        {
            Console.Log("Failed to load text file '" + bootTextFileName + "'. Ensure it's in the Resources folder.");
        }
    }

    private IEnumerator DisplayNextLine()
    {
        while (lineIndex < bootText.Count)
        {
            currentLine = bootText[lineIndex];
            lineIndex++;
            currentChar = 0;
            AddNewline();

            while (currentChar < currentLine.Length)
            {
                typingAccumulator += Time.deltaTime * typingSpeedCharactersPerSecond;
                int charsToAdd = Mathf.FloorToInt(typingAccumulator);
                typingAccumulator -= charsToAdd;

                charsToAdd = Mathf.Min(charsToAdd, currentLine.Length - currentChar);

                if (charsToAdd > 0)
                {
                    displayedLines[displayedLines.Count - 1] += currentLine.Substring(currentChar, charsToAdd);
                    currentChar += charsToAdd;
                    bootTextBox.text = string.Join("\n", displayedLines);
                }
                yield return null;
            }

            float lineDelaySeconds = Random.Range(minLineDelaySeconds, maxLineDelaySeconds);
            yield return new WaitForSeconds(lineDelaySeconds);
        }

        Console.Log("Finished displaying all lines.");

        yield return new WaitForSeconds(delayAfterLastLineSeconds);
        onBootSequenceComplete?.Invoke();
    }

    private void AddNewline()
    {
        displayedLines.Add("");
        if (displayedLines.Count > lineLimit)
        {
            displayedLines.RemoveAt(0);
        }
        bootTextBox.text = string.Join("\n", displayedLines);
    }
}
