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
    if (body_text is string singleText)
    {
        return singleText;
    }
    else if (body_text is List<object> textList)
    {
        if (textList.Count > 0)
        {
            int randomIndex = Random.Range(0, textList.Count);
            var randText = textList[randomIndex].ToString();
            id += $"_{randomIndex}";
            Debug.Log($"Random body text (List<object>): {randText}, New ID: {id}");
            return randText;
        }
    }
    else if (body_text is object[] textArray)
    {
        if (textArray.Length > 0)
        {
            int randomIndex = Random.Range(0, textArray.Length);
            var randText = textArray[randomIndex].ToString();
            id += $"_{randomIndex}";
            Debug.Log($"Random body text (object[]): {randText}, New ID: {id}");
            return randText;
        }
    }
    else if (body_text is Newtonsoft.Json.Linq.JArray jsonArray)
    {
        if (jsonArray.Count > 0)
        {
            int randomIndex = Random.Range(0, jsonArray.Count);
            var randText = jsonArray[randomIndex].ToString();
            id += $"_{randomIndex}";
            Debug.Log($"Random body text (JArray): {randText}, New ID: {id}");
            return randText;
        }
    }

    Debug.LogError("Something went wrong with body_text deserialization: " + body_text);
    return "Something went wrong here...";
}



    [System.Serializable]
    public class Choice
    {
        public string text;
        public string next_node;
        public string button;
    }
}
