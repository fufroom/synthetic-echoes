using UnityEngine;
using UnityEngine.Events;

public class LerpOnStart : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 targetPosition;
    public float duration = 1.0f;

    public UnityEvent onLerpStart;
    public UnityEvent onLerpEnd;

    private float elapsedTime = 0.0f;
    private bool isLerping = false;

    void Start()
    {
        transform.localPosition = startPosition;
    }

    public void StartLerp()
    {
        elapsedTime = 0.0f;
        isLerping = true;
        onLerpStart?.Invoke();
    }

    void Update()
    {
        if (isLerping)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);

            if (t >= 1.0f)
            {
                isLerping = false;
                onLerpEnd?.Invoke();
            }
        }
    }
}
