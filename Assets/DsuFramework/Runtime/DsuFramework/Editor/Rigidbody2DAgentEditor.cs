#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Dsu.Framework;

namespace Dsu.Framework
{
    [CustomEditor(typeof(Rigidbody2DAgent))]
    public class Rigidbody2DAgentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 상단 HelpBox
            EditorGUILayout.HelpBox(
                "This GameObject or one of its parent objects must have a Rigidbody2DManager component.",
                MessageType.Info);

            // 기본 인스펙터 그리기
            DrawDefaultInspector();
        }
    }
}
#endif
