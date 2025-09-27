using UnityEngine;
using TMPro;

public class FPSDisplayTMP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float updateInterval = 0.5f;

    private float accumulated = 0f;
    private int frames = 0;
    private float timeLeft;
    private float fps;

    void Start()
    {
        if (fpsText == null)
        {
            Debug.LogError("FPSDisplayTMP: Assign a TextMeshProUGUI reference in the inspector!");
            enabled = false;
            return;
        }

        timeLeft = updateInterval;
    }

    void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;
        timeLeft -= deltaTime;
        accumulated += 1f / deltaTime;
        frames++;

        if (timeLeft <= 0f)
        {
            fps = accumulated / frames;
            timeLeft = updateInterval;
            accumulated = 0f;
            frames = 0;

            fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";
        }
    }
}
