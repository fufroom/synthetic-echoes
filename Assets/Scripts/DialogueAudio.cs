using UnityEngine;
using System.Collections;

public class DialogueAudio : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlayVoice(string nodeId, System.Action onComplete)
    {
        string[] possiblePaths =
        {
            $"sounds/voice/voice_{nodeId}",
            $"sounds/voice/MainPrompts_{nodeId}",
            $"sounds/voice/PromptResponses_{nodeId}",
            $"sounds/voice/Continue_{nodeId}"
        };

        AudioClip clip = null;
        foreach (string path in possiblePaths)
        {
            clip = Resources.Load<AudioClip>(path);
            if (clip != null)
            {
                Debug.Log($"Playing voice file: {path}");
                break;
            }
        }

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            StartCoroutine(WaitForAudio(onComplete));
        }
        else
        {
            Debug.Log($"No voice file found for node ID: {nodeId} in any expected format.");
            onComplete?.Invoke();
        }
    }

    IEnumerator WaitForAudio(System.Action onComplete)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        onComplete?.Invoke();
    }
}
