using System;
using System.Globalization;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FPSAndCPUUsageDisplay : MonoBehaviour
{
    public float updateInterval = 0.5f; // Time interval for updating metrics
    private float accum = 0.0f; // FPS accumulated time
    private int frames = 0; // Frame count
    private float timeleft; // Left time for current interval

    private TextMeshProUGUI fpsText;
    private TextMeshProUGUI cpuText;

    [Obsolete("Obsolete")]
    private void Start()
    {
        // Create Canvas
        Canvas canvas = CreateCanvas();

        // Create FPS Text
        fpsText = CreateTextElement(canvas, "FPS: 0", new Vector2(10, 10));

        // Create CPU Text
        cpuText = CreateTextElement(canvas, "CPU: 0%", new Vector2(10, 40));
    }

    private void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update metrics
        if (timeleft <= 0.0)
        {
            // Display FPS
            float fps = accum / frames;
            fpsText.text = "FPS: " + Mathf.Round(fps).ToString(CultureInfo.InvariantCulture);

            // Display CPU Usage
            float cpuUsage = GetCPUUsage();
            cpuText.text = "CPU: " + Mathf.Round(cpuUsage * 100).ToString(CultureInfo.InvariantCulture) + "%";

            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    [Obsolete("Obsolete")]
    private Canvas CreateCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            canvas = new GameObject("MetricsCanvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<CanvasScaler>();
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }
        return canvas;
    }

    private TextMeshProUGUI CreateTextElement(Canvas canvas, string text, Vector2 position)
    {
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(canvas.transform, false);
        TextMeshProUGUI textMeshPro = textObject.AddComponent<TextMeshProUGUI>();
        textMeshPro.text = text;
        textMeshPro.fontSize = 24;
        textMeshPro.color = Color.white;
        textMeshPro.alignment = TextAlignmentOptions.TopLeft;
        textMeshPro.rectTransform.anchoredPosition = position;
        return textMeshPro;
    }

    private float GetCPUUsage()
    {
        float cpuUsage = 0.0f;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
        cpuUsage = (float)(process.TotalProcessorTime.TotalMilliseconds / (Environment.TickCount / 1000.0f));
#endif
        return cpuUsage;
    }
}