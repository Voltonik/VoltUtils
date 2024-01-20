using UnityEngine;

namespace Volt.Utils.Debug {
    public class PlayerConsole : MonoBehaviour {
        public LogSeverity LogLevel;

        private LogSeverity m_previousLogLevel;

        private void Start() {
            var consoleUI = Instantiate(Resources.Load<ConsoleGUI>("Prefabs/ConsoleGUI"));
            DontDestroyOnLoad(consoleUI);

            Console.SetLogLevel(LogLevel);
            Console.Init(consoleUI);

            Console.SetOpen(false);

            ConfigVar.Init();
        }

        private void OnDestroy() {
            Console.Shutdown();
        }

        private void Update() {
            if (m_previousLogLevel != LogLevel) {
                Console.SetLogLevel(LogLevel);
                m_previousLogLevel = LogLevel;
            }

            Console.ConsoleUpdate();

            m_previousLogLevel = LogLevel;
        }

        private void LateUpdate() {
            Console.ConsoleLateUpdate();
        }
    }
}