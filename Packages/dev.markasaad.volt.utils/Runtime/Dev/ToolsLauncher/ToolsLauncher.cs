using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Volt.Utils.Extensions;

namespace Volt.Utils.Dev {
    public class ToolsLauncher : MonoBehaviour {
        [SerializeField] private Transform m_toolsParent;
        [SerializeField] private GameObject m_toolPrefab;
        [SerializeField] private GameObject m_toolsView;

        public Dictionary<string, Action> Tools = new() {
            { "Console", () => Volt.Utils.Dev.Console.SetOpen(true) }
        }; 

        private void Start() {
            ReinitializeTools();
        }

        private void ReinitializeTools() {
            foreach (var child in m_toolsParent.Children()) {
                Destroy(child.gameObject);
            }
            foreach (var tool in Tools) {
                var b = Instantiate(m_toolPrefab, m_toolsParent).GetComponent<Button>();
                b.onClick.AddListener(() => {
                    tool.Value.Invoke();
                    m_toolsView.SetActive(false);
                });
                b.gameObject.GetComponentInChildren<Text>().text = tool.Key;
            }
        }

        public void ToggleLauncher() {
            m_toolsView.SetActive(!m_toolsView.activeSelf);
        }
    }
}