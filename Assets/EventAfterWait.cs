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

    private Coroutine waitCoroutine;
    private bool isCancelled = false;

    private void Start()
    {
        if (triggerOnStart)
        {
            StartWaiting();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("User skip activated");
            CancelWaiting();
            onWaitCompleted?.Invoke();
        }
    }

    /// <summary>
    /// Starts the wait timer and triggers the event after the specified time.
    /// </summary>
    public void StartWaiting()
    {
        isCancelled = false;
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
        }
        waitCoroutine = StartCoroutine(WaitAndTrigger());
    }

    /// <summary>
    /// Cancels the waiting coroutine if it is running.
    /// </summary>
    public void CancelWaiting()
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
            isCancelled = true;
        }
    }

    private IEnumerator WaitAndTrigger()
    {
        yield return new WaitForSeconds(waitTime);
        
        if (!isCancelled)
        {
            onWaitCompleted?.Invoke();
        }

        waitCoroutine = null;
    }
}
