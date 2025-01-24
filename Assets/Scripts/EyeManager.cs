using System.Collections;
using UnityEngine;

public class EyeManager : MonoBehaviour
{
    public MaterialToggle leftEye; // Reference to the left eye's MaterialToggle
    public MaterialToggle rightEye; // Reference to the right eye's MaterialToggle

    // Turn both eyes on
    public void TurnOnEyes()
    {
        if (leftEye != null) leftEye.SetToggleState(true);
        if (rightEye != null) rightEye.SetToggleState(true);
    }

    // Turn both eyes off
    public void TurnOffEyes()
    {
        if (leftEye != null) leftEye.SetToggleState(false);
        if (rightEye != null) rightEye.SetToggleState(false);
    }

    // Flicker both eyes for a specified duration and frequency
    public void FlickerEyes(float duration, float frequency)
    {
        StartCoroutine(FlickerRoutine(duration, frequency));
    }

    private IEnumerator FlickerRoutine(float duration, float frequency)
    {
        float elapsedTime = 0f;
        bool toggleState = false;

        while (elapsedTime < duration)
        {
            toggleState = !toggleState;

            if (leftEye != null) leftEye.SetToggleState(toggleState);
            if (rightEye != null) rightEye.SetToggleState(toggleState);

            yield return new WaitForSeconds(frequency);
            elapsedTime += frequency;
        }

        // Ensure eyes are turned off at the end of flicker
        TurnOffEyes();
    }
}
