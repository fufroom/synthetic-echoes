using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;
    public object body_text;
    public List<Choice> choices;
    public bool text_to_speech;
    public string play_sound;
    public string animation;

   public string GetBodyText()
{
    if (body_text == null)
    {
        Debug.LogWarning($"DialogueNode {id} has no body_text field.");
        return "Missing text";
    }

    if (body_text is string singleText)
    {
        return singleText;
    }
    else if (body_text is List<object> textList && textList.Count > 0)
    {
        return textList[Random.Range(0, textList.Count)].ToString();
    }
    else if (body_text is List<string> stringList && stringList.Count > 0)
    {
        return stringList[Random.Range(0, stringList.Count)];
    }
    Debug.LogError($"Unexpected body_text format in node {id}");
    return "Invalid format";
}

    [System.Serializable]
    public class Choice
    {
        public string text;
        public string next_node;
        public string button;
    }
}
