using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Collections;
using System;

public class SubwooferController : MonoBehaviour
{
    public Image connectionIcon;
    private SerialPort serialPort = new SerialPort("/dev/ttyACM0", 9600);
    private bool isConnected = false;

    void Start()
    {
        try
        {
            serialPort.Open();
            isConnected = serialPort.IsOpen;
            UpdateIconColor();
        }
        catch (Exception)
        {
            isConnected = false;
            UpdateIconColor();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3)) SendFrequency(50);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SendFrequencySequence(new int[] { 50, 20, 50, 20, 50 }, 1000);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SendFrequencySequence(new int[] { 30, 50, 70, 90, 110 }, 2000);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SendFrequencySequence(new int[] { 100, 90, 80, 70, 60 }, 4000);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SendFrequencySequence(new int[] { 40, 80, 120, 80, 40 }, 6000);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SendFrequencySequence(new int[] { 50, 55, 60, 65, 70 }, 4000);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SendGlitchTone(50, 600, 5000, 200);
        if (Input.GetKeyDown(KeyCode.Alpha0)) SendRandomFrequencySequence(8, 10, 100, 2000);
        if (Input.GetKeyDown(KeyCode.Space)) SendFrequency(0);
    }

    void SendFrequency(int frequency)
    {
        if (serialPort.IsOpen)
        {
            serialPort.WriteLine(frequency.ToString());
            StartCoroutine(FlashIconColor(Color.yellow, 1));
        }
    }

    void SendFrequencySequence(int[] frequencies, int duration)
    {
        StartCoroutine(PlayToneSequence(frequencies, duration));
    }

    IEnumerator PlayToneSequence(int[] frequencies, int duration)
    {
        int interval = duration / frequencies.Length;
        foreach (int freq in frequencies)
        {
            SendFrequency(freq);
            yield return new WaitForSeconds(interval / 1000f);
        }
        SendFrequency(0);
    }

    void SendGlitchTone(int minFreq, int maxFreq, int duration, int interval)
    {
        if (serialPort.IsOpen)
        {
            serialPort.WriteLine($"GLITCH {minFreq} {maxFreq} {duration} {interval}");
            StartCoroutine(FlashIconColor(Color.yellow, 1));
        }
    }

    void SendRandomFrequencySequence(int count, int minFreq, int maxFreq, int duration)
    {
        int[] randomFrequencies = new int[count];
        for (int i = 0; i < count; i++)
        {
            randomFrequencies[i] = UnityEngine.Random.Range(minFreq, maxFreq);
        }
        SendFrequencySequence(randomFrequencies, duration);
    }

    IEnumerator FlashIconColor(Color color, float duration)
    {
        if (connectionIcon != null)
        {
            connectionIcon.color = color;
            yield return new WaitForSeconds(duration);
            UpdateIconColor();
        }
    }

    void UpdateIconColor()
    {
        if (connectionIcon != null)
        {
            connectionIcon.color = isConnected ? Color.white : Color.black;
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
