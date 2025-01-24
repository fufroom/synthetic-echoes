using UnityEngine;

public class MouthController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    [Range(0.1f, 10.0f)] public float sensitivity = 1.0f; // Adjusts responsiveness to audio
    [Range(0.0f, 1.0f)] public float threshold = 0.05f;  // Minimum value to trigger visuals

    [Header("Emission Settings")]
    [Range(0.0f, 10.0f)] public float maxEmissionIntensity = 5.0f; // Maximum emissive intensity
    [Range(5f, 15.0f)] public float fadeSpeed = 1.0f; // Fade speed when audio stops
    [Range(0.01f, 0.5f)] public float smoothness = 0.1f; // Smooth transition effect

    private Renderer[] childRenderers;
    private float[] samples = new float[512];
    private float[] frequencyBands = new float[6];
    private float[] bandBuffer = new float[6];
    private float[] bufferDecrease = new float[6];
    private Color baseEmissionColor;

    void Start()
    {
        // Get all child objects with a Renderer component
        childRenderers = GetComponentsInChildren<Renderer>();

        if (childRenderers.Length != 6)
        {
            Debug.LogError("MouthController requires exactly 6 child objects with Renderer components.");
        }

        // Get the default emissive color of the materials (assuming they are all the same)
        if (childRenderers.Length > 0)
        {
            baseEmissionColor = childRenderers[0].material.GetColor("_EmissionColor");
        }
    }

    void Update()
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            FadeToBlack();
            return;
        }

        AnalyzeAudio();
        UpdateBandBuffer();
        ApplyEqualizerVisualization();
    }

    void AnalyzeAudio()
    {
        // Get spectrum data from the audio source
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);

        // Divide the spectrum into 6 frequency bands
        int count = 0;
        for (int i = 0; i < frequencyBands.Length; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if (i == frequencyBands.Length - 1)
            {
                sampleCount += 2; // Add remaining samples to the last band
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count + 1);
                count++;
            }

            average /= count;
            frequencyBands[i] = Mathf.Max(0, average * sensitivity - threshold); // Apply sensitivity and threshold
        }
    }

    void UpdateBandBuffer()
    {
        // Smooth the frequency response with a buffer
        for (int i = 0; i < frequencyBands.Length; i++)
        {
            if (frequencyBands[i] > bandBuffer[i])
            {
                bandBuffer[i] = frequencyBands[i];
                bufferDecrease[i] = smoothness;
            }
            else
            {
                bandBuffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f; // Increase buffer falloff speed
            }
        }
    }

    void ApplyEqualizerVisualization()
    {
        for (int i = 0; i < childRenderers.Length; i++)
        {
            float intensity = Mathf.Lerp(0, maxEmissionIntensity, bandBuffer[i]);
            SetEmission(childRenderers[i], intensity);
        }
    }

    void SetEmission(Renderer renderer, float intensity)
    {
        if (renderer != null)
        {
            Color emissionColor = baseEmissionColor * Mathf.LinearToGammaSpace(intensity);
            renderer.material.SetColor("_EmissionColor", emissionColor);
        }
    }

    void FadeToBlack()
    {
        // Gradually fade back to black when no audio is playing
        foreach (Renderer renderer in childRenderers)
        {
            Color currentEmission = renderer.material.GetColor("_EmissionColor");
            Color targetEmission = baseEmissionColor * Mathf.LinearToGammaSpace(0);
            renderer.material.SetColor("_EmissionColor", Color.Lerp(currentEmission, targetEmission, fadeSpeed * Time.deltaTime));
        }
    }
}
