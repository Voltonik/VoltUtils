using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;


namespace Volt.Utils.Dev {
    [Flags]
    public enum ConfigVarFlags {
        None = 0x0,       // None
        Save = 0x1,       // Causes the cvar to be save to settings.cfg
        Cheat = 0x2,      // Consider this a cheat var. Can only be set if cheats enabled
        ServerInfo = 0x4, // These vars are sent to clients when connecting and when changed
        ClientInfo = 0x8, // These vars are sent to server when connecting and when changed
        User = 0x10,      // User created variable
    }

    public class ConfigVarAttribute : Attribute {
        public string Name = null;
        public string DefaultValue = "";
        public ConfigVarFlags Flags = ConfigVarFlags.Save;
        public string Description = "";
    }

    public class ConfigVar {
        public readonly string Name;
        public readonly string Description;
        public readonly string DefaultValue;
        public readonly ConfigVarFlags Flags;
        public bool Changed;

        private string m_stringValue;
        private float m_floatValue;
        private int m_intValue;

        public static Dictionary<string, ConfigVar> ConfigVars;
        private static bool s_initialized = false;

        public static ConfigVarFlags DirtyFlags { get; set; } = ConfigVarFlags.None;
        private static readonly Regex ValidateNameRe = new Regex(@"^[a-z_+-][a-z0-9_+.-]*$");

        public static string CONFIG_FILE_NAME = "settings.cfg";
        public static string CONFIG_FILE_DIRECTORY = Application.persistentDataPath;

        public virtual string Value {
            get { return m_stringValue; }
            set {
                if (m_stringValue == value)
                    return;
                DirtyFlags |= Flags;
                m_stringValue = value;
                if (!int.TryParse(value, out m_intValue))
                    m_intValue = 0;
                if (!float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out m_floatValue))
                    m_floatValue = 0;
                Changed = true;
            }
        }

        public int IntValue {
            get { return m_intValue; }
        }

        public float FloatValue {
            get { return m_floatValue; }
        }

        public ConfigVar(string name, string description, string defaultValue, ConfigVarFlags flags = ConfigVarFlags.Save) {
            Name = name;
            Flags = flags;
            Description = description;
            DefaultValue = defaultValue;
        }

        public static void Init() {
            if (s_initialized)
                return;

            ConfigVars = new Dictionary<string, ConfigVar>();
            InjectAttributeConfigVars();
            s_initialized = true;
        }

        public static void ResetAllToDefault() {
            foreach (var v in ConfigVars) {
                v.Value.LoadValueOrDefault();
            }
        }

        public static void SaveChangedVars() {
            if ((DirtyFlags & ConfigVarFlags.Save) == ConfigVarFlags.None)
                return;

            Save();
        }

        public static void Save() {
            string path = System.IO.Path.Join(CONFIG_FILE_DIRECTORY, CONFIG_FILE_NAME);

            using (var st = System.IO.File.CreateText(path)) {
                foreach (var cvar in ConfigVars.Values) {
                    if ((cvar.Flags & ConfigVarFlags.Save) == ConfigVarFlags.Save) {
                        st.WriteLine("{0} \"{1}\"", cvar.Name, cvar.Value);
                    }
                }
                DirtyFlags &= ~ConfigVarFlags.Save;
            }

            Debug.Log("Config saved");
        }

        public static void RegisterConfigVar(ConfigVar cvar) {
            if (ConfigVars.ContainsKey(cvar.Name)) {
                Debug.LogError($"Trying to register cvar \"{cvar.Name}\" twice");
                return;
            }
            if (!ValidateNameRe.IsMatch(cvar.Name)) {
                Debug.LogError($"Trying to register cvar with invalid name: \"{cvar.Name}\""
                + "\nthe name must be in lowercase, begin with a letter or (_ + -), and end with a letter or a number or (_ + . -)");
                return;
            }

            ConfigVars.Add(cvar.Name, cvar);
        }

        private static void InjectAttributeConfigVars() {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var @class in assembly.GetTypes()) {
                    if (!@class.IsClass)
                        continue;
                    foreach (var field in @class.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)) {
                        if (!field.IsDefined(typeof(ConfigVarAttribute), false))
                            continue;
                        if (!field.IsStatic) {
                            Debug.LogError("Cannot use ConfigVar attribute on non-static fields");
                            continue;
                        }
                        if (field.FieldType != typeof(ConfigVar)) {
                            Debug.LogError("Cannot use ConfigVar attribute on fields not of type ConfigVar");
                            continue;
                        }
                        var attr = field.GetCustomAttributes(typeof(ConfigVarAttribute), false)[0] as ConfigVarAttribute;
                        var name = attr.Name ?? @class.Name.ToLower() + "." + field.Name.ToLower();
                        if (field.GetValue(null) is ConfigVar) {
                            Debug.LogError("ConfigVars (" + name + ") should not be initialized from code; just marked with attribute");
                            continue;
                        }
                        ConfigVar cvar = new ConfigVar(name, attr.Description, attr.DefaultValue, attr.Flags);
                        cvar.LoadValueOrDefault();
                        RegisterConfigVar(cvar);
                        field.SetValue(null, cvar);
                    }
                }
            }

            // Clear dirty flags as default values shouldn't count as dirtying
            DirtyFlags = ConfigVarFlags.None;
        }

        private void LoadValueOrDefault() {
            Value = DefaultValue;

            string path = System.IO.Path.Join(CONFIG_FILE_DIRECTORY, CONFIG_FILE_NAME);

            if (System.IO.File.Exists(path)) {
                using var st = System.IO.File.OpenText(path);
                string line;
                while ((line = st.ReadLine()) != null) {
                    string[] tokens = line.Split(" ");

                    if (tokens[0] == Name) {
                        Value = tokens[1].Trim('"');
                    }
                }
                Debug.Log("Config loaded");
            }
        }

        public bool ChangeCheck() {
            if (!Changed)
                return false;
            Changed = false;
            return true;
        }
    }
}