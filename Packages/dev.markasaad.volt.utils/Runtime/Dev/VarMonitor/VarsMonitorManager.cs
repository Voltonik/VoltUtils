using System.Reflection;

using UnityEngine;

namespace Volt.Utils.Dev {
    public unsafe class VarsMonitorManager : MonoBehaviour {
        private float test = 2;

        private void Start() {
            var monitorUI = Instantiate(Resources.Load<VarsMonitorGUI>("Prefabs/VarsMonitorGUI"));
            DontDestroyOnLoad(monitorUI);

            VarsMonitor.Init(monitorUI);
            VarsMonitor.SetOpen(true);

            fixed (float* testPtr = &test) {
                VarsMonitor.AddVar(new MonitoredVar("test", testPtr, typeof(float)));
            }
        }

        private void Update() {
            test = Time.deltaTime;
        }
    }
}