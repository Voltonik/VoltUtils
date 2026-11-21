using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.Layouts;
#endif

namespace Volt.Utils.Dev {
    public class ConsoleManager : MonoBehaviour {
#if ENABLE_INPUT_SYSTEM
        [Tooltip("The key, or any other button, that opens and closes the console.")]
        [InputControl(layout = "Button")]
        [SerializeField] private string m_toggleKey = "<Keyboard>/f1";
#endif

        private void Start() {
            var consoleUI = Instantiate(Resources.Load<ConsoleGUI>("Prefabs/ConsoleGUI"));
            DontDestroyOnLoad(consoleUI.gameObject);
            var toolsLauncher = Instantiate(Resources.Load<GameObject>("Prefabs/ToolsGUI"));
            DontDestroyOnLoad(toolsLauncher);

#if ENABLE_INPUT_SYSTEM
            consoleUI.ToggleBinding = m_toggleKey;
#endif
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
