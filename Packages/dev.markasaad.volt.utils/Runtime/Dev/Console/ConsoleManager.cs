using UnityEngine;

namespace Volt.Utils.Dev {
    public class ConsoleManager : MonoBehaviour {
        private void Start() {
            var consoleUI = Instantiate(Resources.Load<ConsoleGUI>("Prefabs/ConsoleGUI"));
            DontDestroyOnLoad(consoleUI.gameObject);
            var toolsLauncher = Instantiate(Resources.Load<GameObject>("Prefabs/ToolsGUI"));
            DontDestroyOnLoad(toolsLauncher);

            ConfigVar.Init();
            Console.Init(consoleUI);

            Console.SetOpen(false);
        }

        private void OnDestroy() {
            Console.Shutdown();
        }

        private void Update() {
            Console.ConsoleUpdate();
        }

        private void LateUpdate() {
            Console.ConsoleLateUpdate();
        }
    }
}