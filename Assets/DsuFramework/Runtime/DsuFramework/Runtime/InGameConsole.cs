using UnityEngine;
using System.Collections.Generic;

namespace Dsu.Framework
{
    public class InGameConsole : MonoBehaviour
    {
        [Header("Settings")]
        public bool consoleEnabled = true;  // Enable 체크박스
        public bool minimized = true;

        private List<string> logs = new List<string>();
        private Vector2 scrollPosition = Vector2.zero;
        private int maxLogCount = 500;

        private Rect windowRect;

        private void Awake()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!consoleEnabled) return;

            logs.Add($"[{type}] {logString}");
            if (logs.Count > maxLogCount) {
                logs.RemoveAt(0);
            }
        }

        void OnGUI()
        {
            if (!consoleEnabled) return;

            GUI.color = new Color(1, 1, 1, 0.8f); // 투명도 (활성화 시)

            float windowHeight = Screen.height * 0.5f;
            windowRect = new Rect(0, Screen.height - windowHeight, Screen.width, windowHeight);

            if (!minimized) {
                windowRect = GUI.Window(0, windowRect, ConsoleWindow, "Console");
            }
            else {
                DrawRestoreButton();
            }
        }

        void ConsoleWindow(int windowID)
        {
            GUILayout.BeginVertical();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(windowRect.height - 70));
            foreach (string log in logs) {
                GUILayout.Label(log);
            }
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Clear")) {
                logs.Clear();
            }

            if (GUILayout.Button("Minimize")) {
                minimized = true;
            }

            if (GUILayout.Button("Disable")) {
                consoleEnabled = false;
            }

            GUILayout.EndHorizontal();

            GUI.DragWindow();
            GUILayout.EndVertical();
        }

        void DrawRestoreButton()
        {
            // Restore 버튼만 화면 하단 중앙에 그리기
            float buttonWidth = 80f;
            float buttonHeight = 20f;
            float xPos = (Screen.width - buttonWidth) / 2f;
            float yPos = Screen.height - buttonHeight - 5f;

            if (GUI.Button(new Rect(xPos, yPos, buttonWidth, buttonHeight), "Console")) {
                minimized = false;
            }
        }
    }
}
