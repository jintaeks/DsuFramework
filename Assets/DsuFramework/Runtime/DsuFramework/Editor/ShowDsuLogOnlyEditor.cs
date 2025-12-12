#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Dsu.Extension
{
    [CustomEditor(typeof(ShowDsuLogOnly))]
    public class ShowDsuLogOnlyEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 박스 그리기를 위한 영역 측정
            Rect startRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true));

            Rect rect = EditorGUILayout.BeginVertical();

            DrawContent();

            EditorGUILayout.EndVertical();

            Rect endRect = GUILayoutUtility.GetLastRect();

            float padding = 6f;
            float top = startRect.yMin - padding;
            float bottom = endRect.yMax + padding;
            float left = 5f;
            float right = EditorGUIUtility.currentViewWidth - 4f;

            Vector3[] linePoints = new Vector3[]
            {
                new Vector3(left, top),
                new Vector3(right, top),
                new Vector3(right, bottom),
                new Vector3(left, bottom),
                new Vector3(left, top)
            };

            Handles.color = Color.red;
            Handles.DrawAAPolyLine(2f, linePoints);
        }

        private void DrawContent()
        {
            ShowDsuLogOnly filter = (ShowDsuLogOnly)target;

            // 기본 필드 출력
            DrawDefaultInspector();

            GUILayout.Space(10);

            // 버튼 공통 스타일
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.padding = new RectOffset(10, 10, 6, 6);

            // Rebuild 버튼 (노란 배경)
            Color originalBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.yellow;

            if (GUILayout.Button("Rebuild", buttonStyle)) {
                ShowDsuLogOnly.Rebuild();
                Debug.Log("[ShowDsuLogOnly] Rebuilt filter class list.");
            }

            GUILayout.Space(5);

            // Remove 버튼 (빨간 배경)
            GUI.backgroundColor = Color.red;

            if (GUILayout.Button("Remove", buttonStyle)) {
                DestroyImmediate(filter);
                return;
            }

            GUI.backgroundColor = originalBackgroundColor;
        }
    }
}
#endif
