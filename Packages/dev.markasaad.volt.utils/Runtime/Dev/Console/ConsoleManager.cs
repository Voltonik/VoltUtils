using UnityEngine;

namespace Volt.Utils.Dev {
    public class ConsoleManager : MonoBehaviour {
        private void Start() {
            var consoleUI = Instantiate(Resources.Load<ConsoleGUI>("Prefabs/ConsoleGUI"));
            DontDestroyOnLoad(consoleUI);

            Console.Init(consoleUI);

            Console.SetOpen(false);

            ConfigVar.Init();
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