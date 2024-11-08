using UnityEngine;

public class MaterialToggle : MonoBehaviour
{
    public Material onMaterial; // Material to use when "on"
    public Material offMaterial; // Material to use when "off"

    private Renderer objectRenderer;
    private bool isOn = false;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        // Set the initial material based on the isOn state
        if (objectRenderer != null)
        {
            objectRenderer.material = isOn ? onMaterial : offMaterial;
        }
    }

    public void SetToggleState(bool isOn)
    {
        this.isOn = isOn;
        if (objectRenderer != null)
        {
            objectRenderer.material = isOn ? onMaterial : offMaterial;
        }
    }
}
