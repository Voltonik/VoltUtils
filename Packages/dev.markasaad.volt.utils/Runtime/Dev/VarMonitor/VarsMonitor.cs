using System;
using System.Collections.Generic;

namespace Volt.Utils.Dev {
    public unsafe class MonitoredVar {
        public string Name;
        public void* Ptr;
        public Type Type;

        public MonitoredVar(string name, void* pointer, Type type) {
            Name = name;
            Ptr = pointer;
            Type = type;
        }

        public string GetValue() {
            return (*(float*)Ptr).ToString();
        }
    }

    public interface IVarsMonitorUI {
        void Init();
        void Shutdown();
        bool IsOpen();
        void SetOpen(bool open);
        void MonitorUpdate();
        void MonitorLateUpdate();
        void AddVar(MonitoredVar var);
        void RemoveVar(MonitoredVar var);
    }

    public class NullVarsMonitor : IVarsMonitorUI {
        public void Init() { }
        public bool IsOpen() => false;
        public void MonitorUpdate() { }
        public void MonitorLateUpdate() { }
        public void AddVar(MonitoredVar var) { }
        public void RemoveVar(MonitoredVar var) { }
        public void SetOpen(bool open) { }
        public void Shutdown() { }
    }

    public class VarsMonitor {
        private static IVarsMonitorUI s_varsMonitorUI = new NullVarsMonitor();
        private static List<MonitoredVar> m_monitoredVars = new List<MonitoredVar>();


        public static bool IsOpen() => s_varsMonitorUI.IsOpen();

        public static void Init(IVarsMonitorUI varsMonitorUI) {
            s_varsMonitorUI = varsMonitorUI;
            s_varsMonitorUI.Init();
        }

        public static void Shutdown() {
            SetOpen(false);
            m_monitoredVars.Clear();
            s_varsMonitorUI.Shutdown();
        }

        public static void MonitorUpdate() {
            if (!IsOpen() || m_monitoredVars.Count == 0)
                return;

            s_varsMonitorUI.MonitorUpdate();
        }

        public static void MonitorLateUpdate() {
            if (!IsOpen() || m_monitoredVars.Count == 0)
                return;

            s_varsMonitorUI.MonitorLateUpdate();
        }

        public static void SetOpen(bool open) {
            s_varsMonitorUI.SetOpen(open);
        }

        public static void AddVar(MonitoredVar var) {
            m_monitoredVars.Add(var);
            s_varsMonitorUI.AddVar(var);
        }

        public static void RemoveVar(MonitoredVar var) {
            m_monitoredVars.Remove(var);
            s_varsMonitorUI.RemoveVar(var);
        }
    }
}