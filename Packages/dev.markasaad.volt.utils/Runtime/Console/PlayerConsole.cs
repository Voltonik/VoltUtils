using UnityEngine;

namespace Volt.Utils.Debug {
    public class PlayerConsole : MonoBehaviour {
        #region Public Fields
        public LogSeverity LogLevel;
        #endregion

        #region Private Fields
        private LogSeverity m_PreviousLogLevel;
        #endregion

        #region Initialization
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
        #endregion

        #region Player Loop
        private void Update() {
            if (m_PreviousLogLevel != LogLevel) {
                Console.SetLogLevel(LogLevel);
                m_PreviousLogLevel = LogLevel;
                VDebug.Log("set log level of console");
            }

            Console.ConsoleUpdate();

            m_PreviousLogLevel = LogLevel;
        }

        private void LateUpdate() {
            Console.ConsoleLateUpdate();
        }
        #endregion
    }
}