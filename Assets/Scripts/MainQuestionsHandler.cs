using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class MainQuestionsHandler : MonoBehaviour
{
    public DialogueManager dialogueManager;
    private List<DialogueNode> generatedNodes = new List<DialogueNode>();
    private int currentNodeIndex = 0;

    private string promptsPath = "Resources/MainPrompts.json";
    private string responsesPath = "Resources/PromptResponses.json";

    public void StartMainQuestionsSequence()
    {
        LoadAndGenerateNodes();
        if (generatedNodes.Count > 0)
        {
            dialogueManager.InjectNodes(generatedNodes);
            dialogueManager.StartNode(generatedNodes[0].id);
        }
    }

    private void LoadAndGenerateNodes()
    {
        List<MainPrompt> prompts = LoadJsonData<MainPrompt>(promptsPath);
        List<PromptResponse> responses = LoadJsonData<PromptResponse>(responsesPath);

        if (prompts == null || responses == null || prompts.Count < 5)
        {
            Debug.LogError("Insufficient data for MainQuestions sequence.");
            return;
        }

        HashSet<int> selectedPromptIndices = new HashSet<int>();
        while (selectedPromptIndices.Count < 5)
        {
            int randomIndex = Random.Range(0, prompts.Count);
            selectedPromptIndices.Add(randomIndex);
        }

        foreach (int index in selectedPromptIndices)
        {
            var prompt = prompts[index];
            var response = responses[Random.Range(0, responses.Count)];

            var promptNode = new DialogueNode
            {
                id = $"MainPrompts_{prompt.id}",
                body_text = prompt.prompt + "\nPlace your left hand on the palm scanner. Then, use your right hand to press the button for option 1 or option 2—choose the one that feels right to you.",
                choices = new List<DialogueNode.Choice>
                {
                    new DialogueNode.Choice { text = "1", button = "1", next_node = $"PromptResponses_{response.id}" },
                    new DialogueNode.Choice { text = "2", button = "2", next_node = $"PromptResponses_{response.id}" }
                },
                text_to_speech = true
            };

            var responseNode = new DialogueNode
            {
                id = $"PromptResponses_{response.id}",
                body_text = response.text,
                choices = new List<DialogueNode.Choice>
                {
                    new DialogueNode.Choice { text = "[1] Continue", button = "1", next_node = "" }
                },
                text_to_speech = true
            };

            generatedNodes.Add(promptNode);
            generatedNodes.Add(responseNode);
        }

        // Link the last response to the "Reflection" node
        generatedNodes[^1].choices[0].next_node = "Reflection";
    }

    private List<T> LoadJsonData<T>(string filePath)
    {
        try
        {
            string fullPath = Path.Combine(Application.dataPath, filePath);
            string json = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading JSON from {filePath}: {ex.Message}");
            return null;
        }
    }

    [System.Serializable]
    public class MainPrompt
    {
        public int id;
        public string prompt;
    }

    [System.Serializable]
    public class PromptResponse
    {
        public string id;
        public string text;
    }
}
