using UnityEngine;
using System.Collections;

public class DialogueAudio : MonoBehaviour
{
    public AudioSource audioSource;
    private bool HasPlayedScannerPrompt = false;

    public void PlayVoice(string nodeId, System.Action onComplete)
    {
        string[] voicePaths = new string[]
        {
            $"sounds/voice/voice_{nodeId}",
            $"sounds/voice/{nodeId}"
        };

        foreach (var path in voicePaths)
        {
            AudioClip clip = Resources.Load<AudioClip>(path);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
                StartCoroutine(PlayClip(clip.length, onComplete));
                Debug.Log($"Playing voice file: {path}");
                return;
            }
        }

        Debug.Log($"No voice file found for {nodeId}");
        onComplete?.Invoke();
    }

    private IEnumerator PlayClip(float duration, System.Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("audioSource.clip.name " + audioSource.clip.name);
        Debug.Log("HasPlayedScannerPrompt " + HasPlayedScannerPrompt);
        if (audioSource.clip.name.Contains("MainPrompts_") && !HasPlayedScannerPrompt)
        {
            AudioClip scannerPrompt = Resources.Load<AudioClip>("sounds/voice/SCANNER_PROMPT");
            if (scannerPrompt != null)
            {
                audioSource.clip = scannerPrompt;
                audioSource.Play();
                HasPlayedScannerPrompt = true;
                yield return new WaitForSeconds(scannerPrompt.length);
            }
        }
        onComplete?.Invoke();
    }
}
