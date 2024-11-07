using UnityEngine;

public class MaterialToggle : MonoBehaviour
{
    public string toggleKeyString = "Space"; // The key to toggle, specified as a string
    public Material onMaterial; // Material to use when "on"
    public Material offMaterial; // Material to use when "off"

    private Renderer objectRenderer;
    private bool isOn = false;
    private KeyCode toggleKey;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        // Convert the string to a KeyCode
        if (System.Enum.TryParse(toggleKeyString, out toggleKey))
        {
            Debug.Log($"Key set to: {toggleKey}");
        }
        else
        {
            Debug.LogWarning($"Invalid key: {toggleKeyString}. Defaulting to Space.");
            toggleKey = KeyCode.Space; // Default to Space if parsing fails
        }

        // Set the initial material based on the isOn state
        if (objectRenderer != null)
        {
            objectRenderer.material = isOn ? onMaterial : offMaterial;
        }
    }

    void Update()
    {
        // Check for the toggle key press
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn; // Toggle the state

            // Apply the correct material based on the new state
            if (objectRenderer != null)
            {
                objectRenderer.material = isOn ? onMaterial : offMaterial;
            }
        }
    }
}
