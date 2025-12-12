#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dsu.Framework
{
    public class Console : MonoBehaviour
    {
        #region Inspector Settings

        [SerializeField] KeyCode toggleKey = KeyCode.BackQuote;
        [SerializeField] bool openOnStart = false;
        [SerializeField] bool shakeToOpen = true;
        [SerializeField] bool shakeRequiresTouch = false;
        [SerializeField] float shakeAcceleration = 3f;
        [SerializeField] float toggleThresholdSeconds = .5f;
        [SerializeField] bool restrictLogCount = false;
        [SerializeField] int maxLogCount = 1000;
        [SerializeField] int logFontSize = 12;
        [SerializeField] float scaleFactor = 1f;
        [SerializeField] bool showTimestamps = true;
        [SerializeField] bool showStackTrace = false;

        #endregion

        static readonly GUIContent clearLabel = new("Clear", "Clear contents of console.");
        static readonly GUIContent onlyLastLogLabel = new("Only Last Log", "Show only most recent log.");
        static readonly GUIContent collapseLabel = new("Collapse", "Hide repeated messages.");
        static readonly GUIContent stackTraceLabel = new("Stack Trace", "Toggle stack trace view.");
        static readonly GUIContent timestampLabel = new("Timestamps", "Toggle timestamps.");

        const int margin = 20;
        const string windowTitle = "Console";

        static readonly Dictionary<LogType, Color> logTypeColors = new()
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.red },
            { LogType.Log, Color.white },
            { LogType.Warning, Color.yellow },
        };

        bool isCollapsed, isVisible, scrollLock, onlyLastLog;
        float lastToggleTime;
        Vector2 scrollPosition;
        float windowX = margin, windowY = margin;

        readonly List<ConsoleLog> logs = new();
        readonly ConcurrentQueue<ConsoleLog> queuedLogs = new();
        readonly Rect titleBarRect = new(0, 0, 10000, 20);

        readonly Dictionary<LogType, bool> logTypeFilters = new()
        {
            { LogType.Assert, true },
            { LogType.Error, true },
            { LogType.Exception, true },
            { LogType.Log, true },
            { LogType.Warning, true },
        };

        void Awake()
        {
#if UNITY_IOS || UNITY_ANDROID
            logFontSize = 18;
            scaleFactor = 1.5f;
#endif
        }

        void Start()
        {
            if (openOnStart) isVisible = true;
        }

        void OnEnable() => Application.logMessageReceivedThreaded += HandleLogThreaded;
        void OnDisable() => Application.logMessageReceivedThreaded -= HandleLogThreaded;

        void Update()
        {
            UpdateQueuedLogs();

            if (Input.GetKeyDown(toggleKey)) isVisible = !isVisible;

            if (shakeToOpen &&
                Input.acceleration.sqrMagnitude > shakeAcceleration &&
                Time.realtimeSinceStartup - lastToggleTime >= toggleThresholdSeconds &&
                (!shakeRequiresTouch || Input.touchCount > 2)) {
                isVisible = !isVisible;
                lastToggleTime = Time.realtimeSinceStartup;
            }
        }

        void OnGUI()
        {
            DrawFloatingToggle();

            if (!isVisible) return;

            GUI.matrix = Matrix4x4.Scale(Vector3.one * scaleFactor);

            float width = (Screen.width / scaleFactor) - (margin * 2);
            float height = (Screen.height / scaleFactor) - (margin * 2);
            Rect windowRect = new(windowX, windowY, width, height);
            Rect newWindowRect = GUILayout.Window(123456, windowRect, DrawWindow, windowTitle);

            windowX = Mathf.Clamp(newWindowRect.x, 0, Screen.width - 100);
            windowY = Mathf.Clamp(newWindowRect.y, 0, Screen.height - 50);
        }

        void DrawFloatingToggle()
        {
            float size = 100f;
            Rect toggleRect = new(Screen.width - size - 10, Screen.height - size - 10, size, 30);

            if (!isVisible && GUI.Button(toggleRect, "¢º Console")) {
                isVisible = true;
            }
        }

        void DrawWindow(int windowID)
        {
            DrawLogList();
            DrawToolbar();
            GUI.DragWindow(titleBarRect);
        }

        void DrawLogList()
        {
            GUIStyle logStyle = GUI.skin.label;
            logStyle.fontSize = logFontSize;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical();

            if (onlyLastLog) {
                var lastLog = GetLastVisibleLog();
                if (lastLog.HasValue) DrawLog(lastLog.Value, logStyle);
            }
            else {
                foreach (var log in logs)
                    if (IsLogVisible(log)) DrawLog(log, logStyle);
            }

            GUILayout.EndVertical();
            Rect inner = GUILayoutUtility.GetLastRect();
            GUILayout.EndScrollView();
            Rect outer = GUILayoutUtility.GetLastRect();

            if (!scrollLock && Event.current.type == EventType.Repaint && IsScrolledToBottom(inner, outer))
                ScrollToBottom();
        }

        void DrawToolbar()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(clearLabel)) logs.Clear();

            foreach (LogType type in Enum.GetValues(typeof(LogType))) {
                logTypeFilters[type] = GUILayout.Toggle(logTypeFilters[type], type.ToString(), GUILayout.ExpandWidth(false));
                GUILayout.Space(10);
            }

            isCollapsed = GUILayout.Toggle(isCollapsed, collapseLabel, GUILayout.ExpandWidth(false));
            onlyLastLog = GUILayout.Toggle(onlyLastLog, onlyLastLogLabel, GUILayout.ExpandWidth(false));
            scrollLock = GUILayout.Toggle(scrollLock, "Scroll Lock", GUILayout.ExpandWidth(false));
            showStackTrace = GUILayout.Toggle(showStackTrace, stackTraceLabel, GUILayout.ExpandWidth(false));
            showTimestamps = GUILayout.Toggle(showTimestamps, timestampLabel, GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();
        }

        void DrawLog(ConsoleLog log, GUIStyle style)
        {
            GUI.contentColor = logTypeColors[log.Type];
            string prefix = showTimestamps ? $"[{log.Timestamp:HH:mm:ss}] " : "";
            string message = prefix + log.GetTruncatedMessage();

            if (isCollapsed) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(message, style);
                GUILayout.FlexibleSpace();
                GUILayout.Label(log.Count.ToString(), GUI.skin.box);
                GUILayout.EndHorizontal();
            }
            else {
                for (int i = 0; i < log.Count; i++) {
                    GUILayout.Label(message, style);
                    if (showStackTrace && !string.IsNullOrEmpty(log.StackTrace))
                        GUILayout.Label(log.StackTrace, GUI.skin.box);
                }
            }

            GUI.contentColor = Color.white;
        }

        void UpdateQueuedLogs()
        {
            while (queuedLogs.TryDequeue(out var log))
                ProcessLogItem(log);
        }

        void HandleLogThreaded(string message, string stackTrace, LogType type)
        {
            queuedLogs.Enqueue(new ConsoleLog
            {
                Count = 1,
                Message = message,
                StackTrace = stackTrace,
                Type = type,
                Timestamp = DateTime.Now
            });
        }

        void ProcessLogItem(ConsoleLog log)
        {
            var lastLog = logs.Count > 0 ? logs[^1] : (ConsoleLog?)null;
            if (lastLog.HasValue && log.Equals(lastLog.Value)) {
                log.Count = lastLog.Value.Count + 1;
                logs[^1] = log;
            }
            else {
                logs.Add(log);
                if (restrictLogCount && logs.Count > maxLogCount)
                    logs.RemoveRange(0, logs.Count - maxLogCount);
            }
        }

        ConsoleLog? GetLastVisibleLog()
        {
            for (int i = logs.Count - 1; i >= 0; i--)
                if (IsLogVisible(logs[i])) return logs[i];
            return null;
        }

        bool IsLogVisible(ConsoleLog log) => logTypeFilters[log.Type];

        bool IsScrolledToBottom(Rect inner, Rect outer) =>
            outer.height >= inner.height ||
            Mathf.Approximately(inner.height, scrollPosition.y + outer.height);

        void ScrollToBottom() => scrollPosition = new Vector2(0, int.MaxValue);
    }

    struct ConsoleLog
    {
        public int Count;
        public string Message;
        public string StackTrace;
        public LogType Type;
        public DateTime Timestamp;

        const int maxMessageLength = 16382;

        public bool Equals(ConsoleLog other) =>
            Message == other.Message && StackTrace == other.StackTrace && Type == other.Type;

        public string GetTruncatedMessage() =>
            string.IsNullOrEmpty(Message) ? Message :
            Message.Length <= maxMessageLength ? Message : Message[..maxMessageLength];
    }

    class ConcurrentQueue<T>
    {
        readonly Queue<T> queue = new();
        readonly object lockObj = new();

        public void Enqueue(T item)
        {
            lock (lockObj) queue.Enqueue(item);
        }

        public bool TryDequeue(out T result)
        {
            lock (lockObj) {
                if (queue.Count == 0) {
                    result = default;
                    return false;
                }

                result = queue.Dequeue();
                return true;
            }
        }
    }
}
#endif
