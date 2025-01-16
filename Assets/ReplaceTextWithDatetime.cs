using UnityEngine;
using TMPro;

public class ReplaceDateTimeText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;

    void Start()
    {
        if (textMeshPro != null)
        {
            string currentDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            textMeshPro.text = textMeshPro.text.Replace("<DATE_TIME>", currentDateTime);
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is not assigned.");
        }
    }
}
