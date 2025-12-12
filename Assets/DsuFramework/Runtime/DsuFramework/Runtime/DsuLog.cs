#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Diagnostics;
using Dsu.Framework;

namespace Dsu
{
    public static class Log
    {
        [Conditional("UNITY_EDITOR")]
        public static void Info(Object context, string message)
        {
            if (!LogSettings.Enabled)
                return;

#if UNITY_EDITOR
            if (ShowDsuLogOnly.HasActiveFilters) {
                if (context == null || !(context is Component comp) || !ShowDsuLogOnly.IsAllowed(comp.gameObject))
                    return;
            }
#endif
            UnityEngine.Debug.Log(FormatMessage(context, message), context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void Warn(Object context, string message)
        {
            if (!LogSettings.Enabled)
                return;

#if UNITY_EDITOR
            if (ShowDsuLogOnly.HasActiveFilters) {
                if (context == null || !(context is Component comp) || !ShowDsuLogOnly.IsAllowed(comp.gameObject))
                    return;
            }
#endif
            UnityEngine.Debug.LogWarning(FormatMessage(context, message), context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void Error(Object context, string message)
        {
            if (!LogSettings.Enabled)
                return;

#if UNITY_EDITOR
            if (ShowDsuLogOnly.HasActiveFilters) {
                if (context == null || !(context is Component comp) || !ShowDsuLogOnly.IsAllowed(comp.gameObject))
                    return;
            }
#endif
            UnityEngine.Debug.LogError(FormatMessage(context, message), context);
        }

        private static string FormatMessage(Object context, string message)
        {
            if (context == null)
                return "[null] " + message;

            return $"[{context.name}] {message}";
        }

        public static void Info(string message)
        {
#if UNITY_EDITOR
            if (!LogSettings.Enabled)
                return;

            if (ShowDsuLogOnly.HasActiveFilters) {
                string caller = GetCallerClassName();
                if (!ShowDsuLogOnly.IsClassAllowed(caller))
                    return;
            }

            UnityEngine.Debug.Log(FormatMessageAuto(message));
#else
            if (!LogSettings.Enabled)
                return;

            UnityEngine.Debug.Log(FormatMessageAuto(message));
#endif
        }

        public static void Warn(string message)
        {
#if UNITY_EDITOR
            if (!LogSettings.Enabled)
                return;

            if (ShowDsuLogOnly.HasActiveFilters) {
                string caller = GetCallerClassName();
                if (!ShowDsuLogOnly.IsClassAllowed(caller))
                    return;
            }

            UnityEngine.Debug.LogWarning(FormatMessageAuto(message));
#else
            if (!LogSettings.Enabled)
                return;

            UnityEngine.Debug.LogWarning(FormatMessageAuto(message));
#endif
        }

        public static void Error(string message)
        {
#if UNITY_EDITOR
            if (!LogSettings.Enabled)
                return;

            if (ShowDsuLogOnly.HasActiveFilters) {
                string caller = GetCallerClassName();
                if (!ShowDsuLogOnly.IsClassAllowed(caller))
                    return;
            }

            UnityEngine.Debug.LogError(FormatMessageAuto(message));
#else
            if (!LogSettings.Enabled)
                return;

            UnityEngine.Debug.LogError(FormatMessageAuto(message));
#endif
        }

        private static string FormatMessageAuto(string message)
        {
            string caller = GetCallerClassName();
            string goName = GetCallerGameObjectName(caller);
            return $"[{caller}] ({goName}) {message}";
        }

        private static string GetCallerClassName()
        {
            var stackTrace = new StackTrace();
            for (int i = 2; i < stackTrace.FrameCount; i++) // skip Log methods
            {
                var method = stackTrace.GetFrame(i).GetMethod();
                if (method.DeclaringType != typeof(Log)) {
                    return method.DeclaringType?.Name ?? "Unknown";
                }
            }
            return "Unknown";
        }
#if UNITY_EDITOR
        private static string GetCallerGameObjectName(string className)
        {
            var behaviours = Object.FindObjectsOfType<MonoBehaviour>();
            foreach (var behaviour in behaviours) {
                if (behaviour != null && behaviour.GetType().Name == className && behaviour.gameObject != null) {
                    return behaviour.gameObject.name;
                }
            }
            return "Unknown";
        }
#else
        private static string GetCallerGameObjectName(string className)
        {
            return "Unavailable";
        }
#endif

    }
}
