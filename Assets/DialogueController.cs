using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public TalkingLights HumanFormMouthTalkingLights;
    public TMP_Text dialogueBoxTitle;
    public TMP_Text dialogueBoxBody;
    
    void Start()
    {
        dialogueBoxTitle.text = "Title Placeholder";
        dialogueBoxBody.text = "Dialogue body text goes here.";
    }

    void Update()
    {
        
    }

   
    public void SetDialogue(string title, string body)
    {
        dialogueBoxTitle.text = title;
        dialogueBoxBody.text = body;
        HumanFormMouthTalkingLights.StartTalking(body);
    }
}
