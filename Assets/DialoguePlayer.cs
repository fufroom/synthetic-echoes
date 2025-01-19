using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Newtonsoft.Json;

public class DialoguePlayer : MonoBehaviour
{
    [System.Serializable]
    public class Choice
    {
        public string text;
        public string next_node;
        public string button;
    }

    [System.Serializable]
    public class DialogueNode
    {
        public string id;
        public string speaker;
        public string body_text;
        public List<Choice> choices;
        public bool text_to_speech;
        public string play_sound;
        public Dictionary<string, string> modify_stat;
        public string animation;
        public string effect_trigger;
        public string vibration_trigger;
    }

    [System.Serializable]
    public class ErrorEntry
    {
        public string id;
        public string message;
    }

    public TMP_Text storyText;
    public TMP_Text speakerText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    public AudioSource audioSource;
    public Animator linkedAnimator;

    private Dictionary<string, DialogueNode> dialogueNodes;
    private DialogueNode currentNode;
    private List<GameObject> currentChoiceButtons;
    private bool inputLocked = false;

    void Start()
    {
        currentChoiceButtons = new List<GameObject>();
        LoadDialogue();

        // Start with the "Start" node
        StartNode("Start");
    }

    void LoadDialogue()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("dialogue");
        if (dialogueJSON == null)
        {
            Debug.LogError("dialogue.json file not found in Resources folder!");
            return;
        }

        List<DialogueNode> nodes = JsonConvert.DeserializeObject<List<DialogueNode>>(dialogueJSON.text);
        dialogueNodes = new Dictionary<string, DialogueNode>();
        foreach (DialogueNode node in nodes)
        {
            dialogueNodes[node.id] = node;
        }
    }

    void Update()
    {
        HandleKeyboardInput();
    }

    void StartNode(string nodeId)
    {
        if (!dialogueNodes.ContainsKey(nodeId))
        {
            Debug.LogError($"Node with ID '{nodeId}' not found.");
            return;
        }

        currentNode = dialogueNodes[nodeId];

        // Handle special SystemReset node
        if (nodeId == "SystemReset")
        {
            HandleSystemResetNode();
        }
        else
        {
            RefreshView();
        }
    }

    void HandleSystemResetNode()
    {
        // Load errors from errors.json
        TextAsset errorsJSON = Resources.Load<TextAsset>("errors");
        if (errorsJSON == null)
        {
            Debug.LogError("errors.json file not found in Resources folder!");
            return;
        }

        List<ErrorEntry> errors = JsonConvert.DeserializeObject<List<ErrorEntry>>(errorsJSON.text);
        if (errors == null || errors.Count == 0)
        {
            Debug.LogError("No errors found in errors.json!");
            return;
        }

        // Pick a random error
        ErrorEntry randomError = errors[Random.Range(0, errors.Count)];
        currentNode.body_text = randomError.message;

        // Set the play_sound path
        currentNode.play_sound = $"sounds/errors/error-{randomError.id}";

        // Display SystemReset-specific button
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
        currentChoiceButtons.Clear();

        GameObject button = Instantiate(choiceButtonPrefab, choiceContainer);
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        buttonText.text = "[1] Reboot";
        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            UnityEngine.Events.UnityEvent rebootEvent = new UnityEngine.Events.UnityEvent();
            rebootEvent.AddListener(RebootSequence);
            rebootEvent.Invoke();
        });

        currentChoiceButtons.Add(button);

        // Play the sound
        PlaySound(currentNode.play_sound);

        // Refresh UI for SystemReset
        storyText.text = currentNode.body_text;
        speakerText.text = currentNode.speaker;
    }

    void RefreshView()
    {
        // Clear existing UI
        storyText.text = currentNode.body_text;
        speakerText.text = currentNode.speaker;

        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
        currentChoiceButtons.Clear();

        // Play voice if text-to-speech is enabled
        if (currentNode.text_to_speech)
        {
            PlayVoice(currentNode.id);
        }

        // Play sound if specified
        if (!string.IsNullOrEmpty(currentNode.play_sound))
        {
            PlaySound(currentNode.play_sound);
        }

        // Handle stats modification
        if (currentNode.modify_stat != null)
        {
            foreach (var stat in currentNode.modify_stat)
            {
                ModifyStat(stat.Key, stat.Value);
            }
        }

        // Trigger animation
        if (!string.IsNullOrEmpty(currentNode.animation))
        {
            linkedAnimator.Play(currentNode.animation);
        }

        // Trigger effects
        if (!string.IsNullOrEmpty(currentNode.effect_trigger) && currentNode.effect_trigger != "none")
        {
            TriggerEffect(currentNode.effect_trigger);
        }

        // Create choice buttons
        foreach (var choice in currentNode.choices)
        {
            GameObject button = Instantiate(choiceButtonPrefab, choiceContainer);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = "[1] " + choice.text;

            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                StartNode(choice.next_node);
                inputLocked = false; // Unlock input after a choice is made
            });

            currentChoiceButtons.Add(button);
        }
    }

    void HandleKeyboardInput()
    {
        if (inputLocked)
        {
            return; // Prevent handling input while locked
        }

        if (currentChoiceButtons.Count >= 1 && Input.GetKeyDown(KeyCode.Alpha1))
        {
            inputLocked = true; // Lock input to prevent holding keys
            currentChoiceButtons[0].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
        }
    }

    void PlayVoice(string nodeId)
    {
        string resourcePath = $"sounds/voice/voice_{nodeId}";
        AudioClip clip = Resources.Load<AudioClip>(resourcePath);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Voice file not found for {resourcePath}");
        }
    }

    void PlaySound(string soundName)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(soundName);
        }
        else
        {
            Debug.LogWarning("SoundManager.Instance is null. Ensure a SoundManager exists in the scene.");
        }
    }

    void ModifyStat(string key, string value)
    {
        int currentValue = PlayerPrefs.GetInt(key, 0);
        int modifyValue = int.Parse(value);
        PlayerPrefs.SetInt(key, currentValue + modifyValue);
    }

    void TriggerEffect(string effectName)
    {
        // Effect triggering logic (placeholder)
    }

    void RebootSequence()
    {
        Debug.Log("Reboot sequence initiated.");
        // Add your Unity event logic for the reboot here.
    }
}
