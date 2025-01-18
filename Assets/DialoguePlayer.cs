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
        RefreshView();
    }

    void RefreshView()
    {
        // Clear UI
        storyText.text = currentNode.body_text;
        speakerText.text = currentNode.speaker;
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
        currentChoiceButtons.Clear();

        // Handle audio
        if (currentNode.text_to_speech)
        {
            PlayVoice(currentNode.id);
        }

        if (!string.IsNullOrEmpty(currentNode.play_sound))
        {
            PlaySound(currentNode.play_sound);
        }

        // Handle stats
        if (currentNode.modify_stat != null)
        {
            foreach (var stat in currentNode.modify_stat)
            {
                ModifyStat(stat.Key, stat.Value);
            }
        }

        // Handle animation
        if (!string.IsNullOrEmpty(currentNode.animation))
        {
            linkedAnimator.Play(currentNode.animation);
        }

        // Handle effects
        if (!string.IsNullOrEmpty(currentNode.effect_trigger) && currentNode.effect_trigger != "none")
        {
            TriggerEffect(currentNode.effect_trigger);
        }

        // Display choices
        foreach (var choice in currentNode.choices)
        {
            GameObject button = Instantiate(choiceButtonPrefab, choiceContainer);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            // Adjust button text based on the button type
            if (choice.button == "space")
            {
                buttonText.text = "[Please scan your palm]";
            }
            else if (choice.button == "1")
            {
                buttonText.text = "[1] " + choice.text;
            }
            else if (choice.button == "2")
            {
                buttonText.text = "[2] " + choice.text;
            }

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

        if (currentChoiceButtons.Count >= 2 && Input.GetKeyDown(KeyCode.Alpha2))
        {
            inputLocked = true; // Lock input to prevent holding keys
            currentChoiceButtons[1].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var button in currentChoiceButtons)
            {
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                if (buttonText.text.ToLower() == "[please scan your palm]")
                {
                    inputLocked = true; // Lock input to prevent holding keys
                    button.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                    break;
                }
            }
        }
    }

    void PlayVoice(string nodeId)
    {
        string resourcePath = $"sounds/voice/{nodeId}";
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
        GameObject effectPrefab = Resources.Load<GameObject>($"Effects/{effectName}");
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab);
        }
        else
        {
            Debug.LogWarning($"Effect prefab not found for {effectName}");
        }
    }
}
