using UnityEngine;
using System.Diagnostics;


namespace unity1week202309.Debug {
    /// <summary>UNITY_EDITOR定義時用関数</summary>
    public class Util {
        //        [DllImport("__Internal")]
        //        private static extern void ConsoleLog(string logString);


        [Conditional("UNITY_EDITOR")]
        public static void Log(string _message) {
            UnityEngine.Debug.Log(_message);
        }


        [Conditional("UNITY_EDITOR")]
        public static void LogFormat(string _message, params object[] _params) {
            UnityEngine.Debug.LogFormat(_message, _params);
        }


        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string _message) {
            UnityEngine.Debug.LogWarning(_message);
        }


        [Conditional("EDITOE_DEBUG")]
        public static void LogWarningFormat(string _message, params object[] _params) {
            UnityEngine.Debug.LogWarningFormat(_message, _params);
        }


        [Conditional("UNITY_EDITOR")]
        public static void LogError(string _message) {
            UnityEngine.Debug.LogError(_message);
        }


        [Conditional("UNITY_EDITOR")]
        public static void LogErrorFormat(string _message, params object[] _params) {
            UnityEngine.Debug.LogErrorFormat(_message, _params);
        }

        public static bool CheckWebGLPlatform() {
            return Application.platform == RuntimePlatform.WebGLPlayer;
        }
    }
} //end Logger