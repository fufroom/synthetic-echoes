using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Newtonsoft.Json;

public class DialoguePlayer : MonoBehaviour
{
    [System.Serializable]
    public class Choice
    {
        public string text;
        public string next_node;
        public string button;
        public string UnityEvent;
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
        public int id;
        public string message;
    }

    public TMP_Text storyText;
    public TMP_Text speakerText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    public AudioSource audioSource;
    public Animator linkedAnimator;
    public UnityEvent SystemReset;

    private Dictionary<string, DialogueNode> dialogueNodes;
    private DialogueNode currentNode;
    private List<GameObject> currentChoiceButtons = new List<GameObject>();
    private List<ErrorEntry> errorMessages = new List<ErrorEntry>();

    void Start()
    {
        LoadDialogue();
        LoadErrorMessages();
        StartNode("Start");
    }

    void LoadDialogue()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("dialogue");
        if (dialogueJSON == null) return;

        List<DialogueNode> nodes = JsonConvert.DeserializeObject<List<DialogueNode>>(dialogueJSON.text);
        dialogueNodes = new Dictionary<string, DialogueNode>();
        foreach (DialogueNode node in nodes)
        {
            dialogueNodes[node.id] = node;
        }
    }

    void LoadErrorMessages()
    {
        TextAsset errorsJSON = Resources.Load<TextAsset>("errors");
        if (errorsJSON != null)
        {
            errorMessages = JsonConvert.DeserializeObject<List<ErrorEntry>>(errorsJSON.text);
        }
    }

    void StartNode(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId) || !dialogueNodes.ContainsKey(nodeId))
        {
            Debug.LogError($"Node with ID '{nodeId}' not found.");
            return;
        }

        currentNode = dialogueNodes[nodeId];
        RefreshView();
    }

    void RefreshView()
    {
        if (currentNode.speaker == "Empathix System Message")
        {
            HandleSystemMessageNode();
            return;
        }

        storyText.text = currentNode.body_text;
        speakerText.text = currentNode.speaker;

        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
        currentChoiceButtons.Clear();

        if (!string.IsNullOrEmpty(currentNode.animation))
        {
            linkedAnimator.Play(currentNode.animation);
        }

        if (currentNode.id == "Start")
        {
            choiceContainer.gameObject.SetActive(true);
        }
        else if (IsCutsceneNode(currentNode))
        {
            PlayCutscene(currentNode);
            return;
        }
        else
        {
            choiceContainer.gameObject.SetActive(false);

            if (currentNode.text_to_speech)
            {
                if (!PlayVoice(currentNode.id))
                {
                    choiceContainer.gameObject.SetActive(true);
                }
            }
            else
            {
                choiceContainer.gameObject.SetActive(true);
            }
        }

        foreach (var choice in currentNode.choices)
        {
            GameObject button = Instantiate(choiceButtonPrefab, choiceContainer);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            if (!string.IsNullOrEmpty(choice.text) && !string.IsNullOrEmpty(choice.button))
            {
                string buttonLabel = choice.button.ToLower() switch
                {
                    "space" => "[Space] ",
                    "1" => "[1] ",
                    "2" => "[2] ",
                    _ => ""
                };

                buttonText.text = buttonLabel + choice.text;
            }

            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(choice.UnityEvent))
                {
                    TriggerUnityEvent(choice.UnityEvent);
                }
                else if (!string.IsNullOrEmpty(choice.next_node))
                {
                    StartNode(choice.next_node);
                }
            });

            currentChoiceButtons.Add(button);
        }
    }

    void HandleSystemMessageNode()
    {
        if (errorMessages.Count == 0)
        {
            Debug.LogError("No error messages available.");
            storyText.text = "System error occurred.";
            choiceContainer.gameObject.SetActive(true);
            return;
        }

        int randomIndex = Random.Range(0, errorMessages.Count);
        ErrorEntry selectedError = errorMessages[randomIndex];

        storyText.text = selectedError.message;

        string errorSoundPath = $"sounds/errors/error-{selectedError.id}";
        AudioClip clip = Resources.Load<AudioClip>(errorSoundPath);

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            StartCoroutine(WaitForCutsceneCompletion(currentNode.choices[0]));
        }
        else
        {
            Debug.LogWarning($"Error sound not found: {errorSoundPath}");
            StartCoroutine(WaitForCutsceneCompletion(currentNode.choices[0]));
        }
    }

    bool PlayVoice(string nodeId)
    {
        string resourcePath = $"sounds/voice/voice_{nodeId}";
        AudioClip clip = Resources.Load<AudioClip>(resourcePath);

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            StartCoroutine(EnableChoicesAfterAudio());
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator EnableChoicesAfterAudio()
    {
        if (audioSource.clip != null)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        choiceContainer.gameObject.SetActive(true);
    }

    void PlayCutscene(DialogueNode node)
    {
        string voicePath = $"sounds/voice/voice_{node.id}";
        AudioClip clip = Resources.Load<AudioClip>(voicePath);

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            StartCoroutine(WaitForCutsceneCompletion(node.choices[0]));
        }
        else
        {
            HandleCutsceneChoice(node.choices[0]);
        }
    }

    private IEnumerator WaitForCutsceneCompletion(Choice choice)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        HandleCutsceneChoice(choice);
    }

    void HandleCutsceneChoice(Choice choice)
    {
        if (!string.IsNullOrEmpty(choice.UnityEvent))
        {
            TriggerUnityEvent(choice.UnityEvent);
        }
        else if (!string.IsNullOrEmpty(choice.next_node))
        {
            StartNode(choice.next_node);
        }
    }

    bool IsCutsceneNode(DialogueNode node)
    {
        return node.choices.Count == 1 && string.IsNullOrEmpty(node.choices[0].text) && string.IsNullOrEmpty(node.choices[0].button);
    }

    void TriggerUnityEvent(string eventName)
    {
        if (eventName == "SystemReset")
        {
            SystemReset?.Invoke();
        }
        else
        {
            Debug.LogWarning($"UnityEvent '{eventName}' not found in DialoguePlayer.");
        }
    }

    public void PlaySound(string soundName)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(soundName);
        }
    }
}
