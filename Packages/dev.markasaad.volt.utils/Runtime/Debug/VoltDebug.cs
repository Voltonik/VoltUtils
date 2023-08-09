using UnityEngine;

namespace Volt.Utils.Debug {
    using Debug = UnityEngine.Debug;

    [System.Flags]
    public enum LogSeverity {
        None = 0,
        [InspectorName("Errors")] Error = 1,
        [InspectorName("Warnings")] Warning = 2,
        Information = 4
    }


    public static class VDebug {
        public const LogSeverity LogEverything = LogSeverity.Information | LogSeverity.Warning | LogSeverity.Error;

        public static void Log(string system, object message, Color distinctColor = default, LogSeverity allowedSeverity = LogEverything, LogSeverity logSeverity = LogSeverity.Information) {
            if (!allowedSeverity.HasFlag(logSeverity))
                return;

            Color logColor = Color.white;

            switch (logSeverity) {
                case LogSeverity.Warning:
                    logColor = Color.yellow;
                    Debug.LogWarning($"[{system}]".AddColor(distinctColor == default ? Color.white : distinctColor) + ":" + $" {message}".AddColor(logColor));
                    break;
                case LogSeverity.Error:
                    logColor = Color.red;
                    Debug.LogError($"[{system}]".AddColor(distinctColor == default ? Color.white : distinctColor) + ":" + $" {message}".AddColor(logColor));
                    break;
                default:
                    Debug.Log($"[{system}]".AddColor(distinctColor == default ? Color.white : distinctColor) + ":" + $" {message}".AddColor(logColor));
                    break;
            }
        }

        public static void LogError(string system, object message, Color distinctColor = default, LogSeverity allowedSeverity = LogEverything) {
            Log(system, message, distinctColor, allowedSeverity, LogSeverity.Error);
        }

        public static void Log(object message) {
            Debug.Log(message);
        }
    }
}