using UnityEngine;
using UnityEngine.Events;

public class EventOnActive : MonoBehaviour
{
    public UnityEvent OnActive;
    void Awake()
    {
        OnActive.Invoke();
    }

 
}
