using System.Collections.Generic;

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

        public static Dictionary<string, Color> LogColors = new Dictionary<string, Color>();

        public static void Log(string system, object message, LogSeverity allowedSeverity = LogEverything, LogSeverity logSeverity = LogSeverity.Information) {
            if (!allowedSeverity.HasFlag(logSeverity))
                return;

            if (!LogColors.TryGetValue(system, out Color distinctColor)) {
                distinctColor = ColorExtensions.RandomColor(system);
                LogColors.Add(system, distinctColor);
            }

            switch (logSeverity) {
                case LogSeverity.Warning:
                    Debug.LogWarning($"[{system}]".AddColor(distinctColor) + ":" + $" {message}".AddColor(Color.yellow));
                    break;
                case LogSeverity.Error:
                    Debug.LogError($"[{system}]".AddColor(distinctColor) + ":" + $" {message}".AddColor(Color.red));
                    break;
                default:
                    Debug.Log($"[{system}]".AddColor(distinctColor) + ":" + $" {message}".AddColor(Color.white));
                    break;
            }
        }

        public static void LogError(string system, object message, LogSeverity allowedSeverity = LogEverything) {
            Log(system, message, allowedSeverity, LogSeverity.Error);
        }

        public static void Log(object message) {
            Debug.Log(message);
        }
    }
}