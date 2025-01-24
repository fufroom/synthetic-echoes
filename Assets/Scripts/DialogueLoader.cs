using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class DialogueLoader : MonoBehaviour
{
    public Dictionary<string, DialogueNode> LoadDialogue()
    {
        try
        {
            string filePath = Path.Combine(Application.dataPath, "Resources/dialogue.json");
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Dialogue file not found at: {filePath}");
                return new Dictionary<string, DialogueNode>();
            }

            string json = File.ReadAllText(filePath);
            List<DialogueNode> nodesList = JsonConvert.DeserializeObject<List<DialogueNode>>(json);

            if (nodesList != null)
            {
                var dialogueNodes = new Dictionary<string, DialogueNode>();
                foreach (var node in nodesList)
                {
                    dialogueNodes[node.id] = node;
                }
                Debug.Log("Dialogue JSON loaded successfully.");
                return dialogueNodes;
            }
            else
            {
                Debug.LogError("Failed to parse dialogue JSON: No valid data found.");
                return new Dictionary<string, DialogueNode>();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading dialogue JSON: {ex.Message}");
            return new Dictionary<string, DialogueNode>();
        }
    }
}
