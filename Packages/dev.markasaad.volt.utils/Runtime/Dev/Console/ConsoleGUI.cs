using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Volt.Utils.Dev {
#if ENABLE_INPUT_SYSTEM
    public class ConsoleGUI : MonoBehaviour, IConsoleUI, ConsoleActionMap.IConsoleActions {
#else
    public class ConsoleGUI : MonoBehaviour, IConsoleUI {
#endif
        [ConfigVar(Name = "console.alpha", DefaultValue = "0.9", Description = "Console transparency.")]
        static ConfigVar consoleAlpha;
        [ConfigVar(Name = "console.textsize", DefaultValue = "14", Description = "Console text font size.")]
        static ConfigVar consoleTextSize;

        private List<string> m_Lines = new List<string>();
        private int m_WantedCaretPosition = -1;

        public Transform panel;
        public InputField input_field;
        public Text text_area;
        public Text caret;
        public Image text_area_background;
        public KeyCode toggle_console_key;
        public Text buildIdText;

        private ConsoleActionMap consoleActionMap;


        void Awake() {
            input_field.onSubmit.AddListener(OnSubmit);
        }

        public void Init() {
            buildIdText.text = Application.version + " (" + Application.unityVersion + ")";

#if ENABLE_INPUT_SYSTEM
            consoleActionMap = new ConsoleActionMap();
            consoleActionMap.Console.AddCallbacks(this);
            consoleActionMap.Enable();
#endif
        }

        private void OnDisable() {
            Shutdown();
        }

        public void Shutdown() {
            consoleActionMap.Disable();
            consoleActionMap.Dispose();
        }

        public void OutputString(string s) {
            m_Lines.Add(s);
            var count = Mathf.Min(100, m_Lines.Count);
            var start = m_Lines.Count - count;
            text_area.text = string.Join("\n", m_Lines.GetRange(start, count).ToArray());
        }

        public void Clear() {
            m_Lines.Clear();
            text_area.text = "";
        }

        public bool IsOpen() {
            return panel.gameObject.activeSelf;
        }

        public void SetOpen(bool open) {
            panel.gameObject.SetActive(open);
            if (open) {
                input_field.ActivateInputField();
            }
        }

        public void ConsoleUpdate() {
#if !ENABLE_INPUT_SYSTEM
            if (Input.GetKeyDown(toggle_console_key) || Input.GetKeyDown(KeyCode.Backslash))
                SetOpen(!IsOpen());
#endif

            if (!IsOpen())
                return;

            var c = text_area_background.color;
            c.a = Mathf.Clamp01(consoleAlpha.FloatValue);
            text_area_background.color = c;

            text_area.fontSize = consoleTextSize.IntValue;
            buildIdText.fontSize = consoleTextSize.IntValue;
            input_field.textComponent.fontSize = consoleTextSize.IntValue + 1;
            caret.fontSize = consoleTextSize.IntValue + 1;

            // This is to prevent clicks outside input field from removing focus
            input_field.ActivateInputField();

#if !ENABLE_INPUT_SYSTEM
            if (Input.GetKeyDown(KeyCode.Tab)) {
                if (input_field.caretPosition == input_field.text.Length && input_field.text.Length > 0) {
                    var res = Console.TabComplete(input_field.text);
                    input_field.text = res;
                    input_field.caretPosition = res.Length;
                }
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                input_field.text = Console.HistoryUp(input_field.text);
                m_WantedCaretPosition = input_field.text.Length;
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                input_field.text = Console.HistoryDown();
                input_field.caretPosition = input_field.text.Length;
            }
#endif
        }

        public void ConsoleLateUpdate() {
            // This has to happen here because keys like KeyUp will navigate the caret
            // int the UI event handling which runs between Update and LateUpdate
            if (m_WantedCaretPosition > -1) {
                input_field.caretPosition = m_WantedCaretPosition;
                m_WantedCaretPosition = -1;
            }
        }


        void OnSubmit(string value) {
            input_field.text = "";
            input_field.ActivateInputField();

            Console.EnqueueCommand(value);
        }

        public void SetPrompt(string prompt) { }

        public void OnToggle(InputAction.CallbackContext context) {
            SetOpen(!IsOpen());
        }

        public void OnAutoComplete(InputAction.CallbackContext context) {
            if (input_field.caretPosition == input_field.text.Length && input_field.text.Length > 0) {
                var res = Console.TabComplete(input_field.text);
                input_field.text = res;
                input_field.caretPosition = res.Length;
            }
        }

        public void OnHistoryUp(InputAction.CallbackContext context) {
            input_field.text = Console.HistoryUp(input_field.text);
            m_WantedCaretPosition = input_field.text.Length;
        }

        public void OnHistoryDown(InputAction.CallbackContext context) {
            input_field.text = Console.HistoryDown();
            input_field.caretPosition = input_field.text.Length;
        }
    }
}