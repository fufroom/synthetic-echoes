using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public DialogueAudio dialogueAudio;
    public DialogueLoader dialogueLoader;
    public MainQuestionsHandler mainQuestionsHandler;

    private Dictionary<string, DialogueNode> dialogueNodes = new Dictionary<string, DialogueNode>();
    private DialogueNode currentNode;

    public void StartDialogue()
    {
        dialogueNodes = dialogueLoader.LoadDialogue();
        if (dialogueNodes.Count > 0)
        {
            StartNode("Start");
        }
        else
        {
            Debug.LogError("Dialogue data is empty. Check JSON file.");
        }
    }

  public void StartNode(string nodeId)
{
    if (string.IsNullOrEmpty(nodeId))
    {
        Debug.LogError("Attempted to start a node with an empty ID.");
        return;
    }

    if (nodeId == "MainQuestions")
    {
        mainQuestionsHandler?.StartMainQuestionsSequence();
        return;
    }

    if (dialogueNodes.ContainsKey(nodeId))
    {
        currentNode = dialogueNodes[nodeId];
        dialogueUI.RefreshView(currentNode, this);
    }
    else
    {
        Debug.LogError($"Node with ID '{nodeId}' not found.");
    }
}
    public void InjectNodes(List<DialogueNode> nodes)
    {
        foreach (var node in nodes)
        {
            dialogueNodes[node.id] = node;
        }
    }

    public void HandleChoiceSelection(string button)
    {
        foreach (var choice in currentNode.choices)
        {
            if (choice.button == button)
            {
                StartNode(choice.next_node);
                return;
            }
        }
        Debug.LogWarning($"No choice found for button {button}");
    }
}
