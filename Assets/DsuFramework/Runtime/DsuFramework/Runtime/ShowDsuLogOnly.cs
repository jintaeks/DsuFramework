#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace Dsu
{
    [DisallowMultipleComponent]
    public class ShowDsuLogOnly : MonoBehaviour
    {
        private static HashSet<GameObject> _activeObjects = new HashSet<GameObject>();
        private static HashSet<string> _allowedClassNames = new HashSet<string>();

        public static bool HasActiveFilters => _activeObjects.Count > 0;

        public static bool IsAllowed(GameObject go)
        {
            return _activeObjects.Contains(go);
        }

        public static bool IsClassAllowed(string className)
        {
            return _allowedClassNames.Contains(className);
        }

        public static void Rebuild()
        {
            _allowedClassNames.Clear();

            foreach (var obj in Object.FindObjectsOfType<ShowDsuLogOnly>()) {
                var components = obj.GetComponents<MonoBehaviour>();
                foreach (var comp in components) {
                    if (comp != null)
                        _allowedClassNames.Add(comp.GetType().Name);
                }
            }
        }

        private void OnEnable()
        {
            _activeObjects.Add(gameObject);
            RegisterClassNames();
        }

        private void OnDisable()
        {
            _activeObjects.Remove(gameObject);
            Rebuild(); // 상태 변화가 있으므로 재생성
        }

        private void RegisterClassNames()
        {
            var components = GetComponents<MonoBehaviour>();
            foreach (var comp in components) {
                if (comp != null)
                    _allowedClassNames.Add(comp.GetType().Name);
            }
        }
    }
}
#endif
