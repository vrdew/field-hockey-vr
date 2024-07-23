using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public float updateInterval = 0.2f;

    private float accum = 0f;
    private int frames = 0;
    private float timeLeft;

    void Start()
    {
        if (fpsText == null)
        {
            Debug.LogError("Please assign a TextMeshProUGUI component to the FPS Counter script!");
            enabled = false;
            return;
        }

        timeLeft = updateInterval;
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeLeft <= 0f)
        {
            float fps = accum / frames;
            fpsText.text = $"FPS: {fps:F2}";

            timeLeft = updateInterval;
            accum = 0f;
            frames = 0;
        }
    }
}