using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEngine.UIElements;

namespace Volt.Utils.Dev {
    [RequireComponent(typeof(UIDocument))]
    public class VarsMonitorGUI : MonoBehaviour, IVarsMonitorUI {
        public class MonitoredVarUI {
            public MonitoredVar MonitoredVar;
            public VisualElement Container;
            public Label Name;
            public Label Value;
        }

        private UIDocument m_uiDocument;
        private VisualElement m_varsScrollView;
        private List<MonitoredVarUI> m_monitoredVarUIs = new List<MonitoredVarUI>();

        public void Init() {
            m_uiDocument = GetComponent<UIDocument>();
            m_varsScrollView = m_uiDocument.rootVisualElement.Q<ScrollView>("vars-scrollview");
        }

        public bool IsOpen() => m_uiDocument.rootVisualElement.style.display == DisplayStyle.Flex;

        public void MonitorLateUpdate() { }

        public void MonitorUpdate() {
            unsafe {
                foreach (var var in m_monitoredVarUIs) {
                    var.Value.text = var.MonitoredVar.GetValue();
                }
            }
        }

        public void AddVar(MonitoredVar var) {
            unsafe {
                var varUI = new MonitoredVarUI {
                    MonitoredVar = var,
                    Container = new VisualElement(),
                    Name = new Label(text: var.Name),
                    Value = new Label(text: var.GetValue())
                };

                varUI.Container.Add(varUI.Name);
                varUI.Container.Add(varUI.Value);
                m_varsScrollView.Add(varUI.Container);

                m_monitoredVarUIs.Add(varUI);
            }
        }

        public void RemoveVar(MonitoredVar var) {
            var varUI = m_monitoredVarUIs.First(v => v.MonitoredVar == var);

            m_varsScrollView.Remove(varUI.Container);
            m_monitoredVarUIs.Remove(varUI);
        }

        public void SetOpen(bool open) {
            m_uiDocument.rootVisualElement.style.display = open ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void Shutdown() {
            m_monitoredVarUIs.Clear();
        }
    }
}