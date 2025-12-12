#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
using System.Diagnostics;

namespace Dsu.Framework
{
    public class PerformanceMonitor : MonoBehaviour
    {
        public enum Corner { TopLeft, TopRight, BottomLeft, BottomRight }
        public Corner displayCorner = Corner.TopLeft;
        public bool displayStats = true;

        private float deltaTime = 0.0f;
        private GUIStyle style;
        private int frameCount = 0;
        private float timePassed = 0f;
        private float cpuUsage = 0f;
        private Process currentProcess;
        private float updateInterval = 1.0f;

        private double lastCpuTime = 0;
        private float lastCpuCheckTime = 0;

        private GUIContent guiContent;
        private int processorCount;

        void Start()
        {
            currentProcess = Process.GetCurrentProcess();
            processorCount = SystemInfo.processorCount;

            style = new GUIStyle();
            style.fontSize = 14;
            style.normal.textColor = Color.white;

            guiContent = new GUIContent();
        }

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            frameCount++;
            timePassed += Time.unscaledDeltaTime;

            if (timePassed >= updateInterval) {
                cpuUsage = GetCPUUsage();
                frameCount = 0;
                timePassed = 0f;
            }
        }

        void OnGUI()
        {
            if (!displayStats) return;

            int width = Screen.width;
            int height = Screen.height;
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;

            long monoUsed = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / (1024 * 1024);
            long monoHeap = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong() / (1024 * 1024);
            long totalReserved = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024 * 1024);
            long totalUsed = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);

            string text = string.Format(
                "FPS: {0:0.} ({1:0.0} ms)\n" +
                "Frame: {2}\n" +
                "Mono Memory: {3} MB / {4} MB\n" +
                "Unity Memory: {5} MB / {6} MB\n" +
                "CPU: {7:0.}% ({8} cores)\n" +
                "GPU: {9}\n" +
                "GPU VRAM: {10} MB",
                fps, msec,
                Time.frameCount,
                monoUsed, monoHeap,
                totalUsed, totalReserved,
                cpuUsage,
                processorCount,
                SystemInfo.graphicsDeviceName,
                SystemInfo.graphicsMemorySize
            );

            guiContent.text = text;
            Vector2 size = style.CalcSize(guiContent);

            float x = 0, y = 0;

            switch (displayCorner) {
            case Corner.TopLeft:
                x = 10; y = 10;
                break;
            case Corner.TopRight:
                x = width - size.x - 10; y = 10;
                break;
            case Corner.BottomLeft:
                x = 10; y = height - size.y - 10;
                break;
            case Corner.BottomRight:
                x = width - size.x - 10; y = height - size.y - 10;
                break;
            }

            // === [Modified Part Starts] ===
            Color originalColor = GUI.color;
            GUI.color = new Color(0, 0, 0, 0.5f); // Semi-transparent black background
            GUI.DrawTexture(new Rect(x - 5, y - 5, size.x + 10, size.y + 10), Texture2D.whiteTexture);
            GUI.color = originalColor;

            GUI.Label(new Rect(x, y, size.x, size.y), guiContent, style);
            // === [Modified Part Ends] ===
        }

        float GetCPUUsage()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            currentProcess.Refresh();

            double currentCpuTime = currentProcess.TotalProcessorTime.TotalMilliseconds;
            float currentTime = Time.realtimeSinceStartup;

            float cpu = (float)((currentCpuTime - lastCpuTime) / ((currentTime - lastCpuCheckTime) * 1000f)) * 100f;

            lastCpuTime = currentCpuTime;
            lastCpuCheckTime = currentTime;

            return cpu / processorCount; // Normalize for logical cores
#else
            return 0f;
#endif
        }

        public void EnableDisplay() => displayStats = true;
        public void DisableDisplay() => displayStats = false;
    }
}
#endif
