using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventAfterWait : MonoBehaviour
{
    [Header("Settings")]
    public float waitTime = 1.0f; // Time to wait before invoking the event
    public bool triggerOnStart = false; // Whether the event should trigger automatically on Start

    [Header("Event")]
    public UnityEvent onWaitCompleted; // Unity Event to trigger after the wait

    private void Start()
    {
        if (triggerOnStart)
        {
            StartWaiting();
        }
    }

    /// <summary>
    /// Starts the wait timer and triggers the event after the specified time.
    /// </summary>
    public void StartWaiting()
    {
        StartCoroutine(WaitAndTrigger());
    }

    private IEnumerator WaitAndTrigger()
    {
        yield return new WaitForSeconds(waitTime);
        onWaitCompleted?.Invoke();
    }
}
