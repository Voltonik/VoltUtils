using UnityEngine;

namespace Volt.Utils.Dev {
    public class VarsMonitorManager : MonoBehaviour {
        private object test = 2;

        private void Start() {
            var monitorUI = Instantiate(Resources.Load<VarsMonitorGUI>("Prefabs/VarsMonitorGUI"));
            DontDestroyOnLoad(monitorUI);

            VarsMonitor.Init(monitorUI);
            VarsMonitor.SetOpen(true);

            VarsMonitor.AddVar(new MonitoredVar("test", test));
        }

        private void Update() {
            test = Time.deltaTime;
        }
    }
}