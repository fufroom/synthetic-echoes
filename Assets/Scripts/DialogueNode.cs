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
            var randText = textList[Random.Range(0, textList.Count)].ToString();
            Debug.Log("Random body text (List<object>): " + randText);
            return randText;
        }
    }
    else if (body_text is object[] textArray)
    {
        if (textArray.Length > 0)
        {
            var randText = textArray[Random.Range(0, textArray.Length)].ToString();
            Debug.Log("Random body text (object[]): " + randText);
            return randText;
        }
    }
    else if (body_text is Newtonsoft.Json.Linq.JArray jsonArray)
    {
        if (jsonArray.Count > 0)
        {
            var randText = jsonArray[Random.Range(0, jsonArray.Count)].ToString();
            Debug.Log("Random body text (JArray): " + randText);
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
