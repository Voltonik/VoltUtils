using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

using Volt.Utils.Debug;

public class ConfigVarAttribute : Attribute {
    public string Name = null;
    public string DefaultValue = "";
    public ConfigVar.Flags Flags = ConfigVar.Flags.Save;
    public string Description = "";
}

public class ConfigVar {
    public readonly string name;
    public readonly string description;
    public readonly string defaultValue;
    public readonly Flags flags;
    public bool changed;

    private string _stringValue;
    private float _floatValue;
    private int _intValue;

    public static Dictionary<string, ConfigVar> ConfigVars;
    public static Flags DirtyFlags = Flags.None;

    static bool s_Initialized = false;

    const string CONFIG_FILE_NAME = "settings.cfg";

    public static void Init() {
        if (s_Initialized)
            return;

        ConfigVars = new Dictionary<string, ConfigVar>();
        InjectAttributeConfigVars();
        s_Initialized = true;
    }

    public static void ResetAllToDefault() {
        foreach (var v in ConfigVars) {
            v.Value.LoadValueOrDefault();
        }
    }

    public static void SaveChangedVars() {
        Debug.Log(DirtyFlags);

        if ((DirtyFlags & Flags.Save) == Flags.None)
            return;

        Save();
    }

    public static void Save() {
        string path = System.IO.Path.Join(Application.persistentDataPath, CONFIG_FILE_NAME);

        using (var st = System.IO.File.CreateText(path)) {
            foreach (var cvar in ConfigVars.Values) {
                if ((cvar.flags & Flags.Save) == Flags.Save) {
                    st.WriteLine("{0} \"{1}\"", cvar.name, cvar.Value);
                    VDebug.Log("Config", $"saved: {cvar.name}: {cvar.Value}", Color.yellow);
                }
            }
            DirtyFlags &= ~Flags.Save;
        }
    }

    private static Regex validateNameRe = new Regex(@"^[a-z_+-][a-z0-9_+.-]*$");
    public static void RegisterConfigVar(ConfigVar cvar) {
        if (ConfigVars.ContainsKey(cvar.name)) {
            VDebug.LogError("Config", $"Trying to register cvar \"{cvar.name}\" twice", Color.yellow);
            return;
        }
        if (!validateNameRe.IsMatch(cvar.name)) {
            VDebug.LogError("Config", $"Trying to register cvar with invalid name: \"{cvar.name}\""
            + "\nthe name must be in lowercase, begin with a letter or (_ + -), and end with a letter or a number or (_ + . -)", Color.yellow);
            return;
        }

        ConfigVars.Add(cvar.name, cvar);
    }

    [Flags]
    public enum Flags {
        None = 0x0,       // None
        Save = 0x1,       // Causes the cvar to be save to settings.cfg
        Cheat = 0x2,      // Consider this a cheat var. Can only be set if cheats enabled
        ServerInfo = 0x4, // These vars are sent to clients when connecting and when changed
        ClientInfo = 0x8, // These vars are sent to server when connecting and when changed
        User = 0x10,      // User created variable
    }

    public ConfigVar(string name, string description, string defaultValue, Flags flags = Flags.Save) {
        this.name = name;
        this.flags = flags;
        this.description = description;
        this.defaultValue = defaultValue;
    }

    public virtual string Value {
        get { return _stringValue; }
        set {
            if (_stringValue == value)
                return;
            DirtyFlags |= flags;
            _stringValue = value;
            if (!int.TryParse(value, out _intValue))
                _intValue = 0;
            if (!float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _floatValue))
                _floatValue = 0;
            changed = true;
        }
    }

    public int IntValue {
        get { return _intValue; }
    }

    public float FloatValue {
        get { return _floatValue; }
    }

    static void InjectAttributeConfigVars() {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach (var _class in assembly.GetTypes()) {
                if (!_class.IsClass)
                    continue;
                foreach (var field in _class.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)) {
                    if (!field.IsDefined(typeof(ConfigVarAttribute), false))
                        continue;
                    if (!field.IsStatic) {
                        VDebug.LogError("Config", "Cannot use ConfigVar attribute on non-static fields", Color.yellow);
                        continue;
                    }
                    if (field.FieldType != typeof(ConfigVar)) {
                        VDebug.LogError("Config", "Cannot use ConfigVar attribute on fields not of type ConfigVar", Color.yellow);
                        continue;
                    }
                    var attr = field.GetCustomAttributes(typeof(ConfigVarAttribute), false)[0] as ConfigVarAttribute;
                    var name = attr.Name != null ? attr.Name : _class.Name.ToLower() + "." + field.Name.ToLower();
                    var cvar = field.GetValue(null) as ConfigVar;
                    if (cvar != null) {
                        VDebug.LogError("Config", "ConfigVars (" + name + ") should not be initialized from code; just marked with attribute", Color.yellow);
                        continue;
                    }
                    cvar = new ConfigVar(name, attr.Description, attr.DefaultValue, attr.Flags);
                    cvar.LoadValueOrDefault();
                    RegisterConfigVar(cvar);
                    field.SetValue(null, cvar);
                }
            }
        }

        // Clear dirty flags as default values shouldn't count as dirtying
        DirtyFlags = Flags.None;
    }

    void LoadValueOrDefault() {
        Value = defaultValue;

        string path = System.IO.Path.Join(Application.persistentDataPath, CONFIG_FILE_NAME);

        if (System.IO.File.Exists(path)) {
            using (var st = System.IO.File.OpenText(path)) {
                string line;
                while ((line = st.ReadLine()) != null) {
                    string[] tokens = line.Split(" ");

                    if (tokens[0] == name) {
                        Value = tokens[1].Trim('"');
                        VDebug.Log("Config", $"Loaded {name}: {Value}", Color.yellow);
                    }
                }
            }
        }
    }

    public bool ChangeCheck() {
        if (!changed)
            return false;
        changed = false;
        return true;
    }
}