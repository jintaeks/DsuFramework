using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dsu.Framework
{
    public class LogController : MonoBehaviour
    {
        [SerializeField]
        private bool _logEnabled = true;
        public bool LogEnabled => _logEnabled;

        // Start is called before the first frame update
        void Awake()
        {
            Debug.unityLogger.logEnabled = _logEnabled;
        }
    }
}