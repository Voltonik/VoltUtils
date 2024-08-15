using UnityEngine;
using System.Collections.Generic;
using System;
using Volt.Utils.Extensions;

namespace Volt.Utils.Dev {
    public interface IConsoleUI {
        void Init();
        void Shutdown();
        void OutputString(string message);
        bool IsOpen();
        void SetOpen(bool open);
        void ConsoleUpdate();
        void ConsoleLateUpdate();
        void SetPrompt(string prompt);
        void Clear();
    }

    public class ConsoleNullUI : IConsoleUI {
        public void ConsoleUpdate() { }
        public void ConsoleLateUpdate() { }
        public void Init() { }
        public void Shutdown() { }
        public bool IsOpen() => false;
        public void OutputString(string message) { }
        public void SetOpen(bool open) { }
        public void SetPrompt(string prompt) { }
        public void Clear() { }
    }

    public class Console {
        private class ConsoleCommand {
            public string Name;
            public MethodDelegate Method;
            public string Description;
            public int Tag;

            public ConsoleCommand(string name, MethodDelegate method, string description, int tag) {
                Name = name;
                Method = method;
                Description = description;
                Tag = tag;
            }
        }

        [ConfigVar(Name = "config.showlastline", DefaultValue = "0", Description = "Show last logged line briefly at top of screen")]
        private static readonly ConfigVar ConsoleShowLastLine;

        [ConfigVar(Name = "config.autosave", DefaultValue = "0", Description = "Auto save variables on change. (ConfigVar Flag must include ConfigVarFlags.Save which is the default Flag)")]
        private static readonly ConfigVar ConsoleAutoSave;
        

        [ConfigVar(Name = "config.log.toggle", DefaultValue = "1", Description = "Output application log messages to the console.")]
        private static readonly ConfigVar ConsoleLogMessages;

        [ConfigVar(Name = "config.log.collapse", DefaultValue = "1", Description = "Output log messages only once. (Recommended)")]
        private static readonly ConfigVar ConsoleLogMessagesCollapse;

        [ConfigVar(Name = "config.log.stacktraces", DefaultValue = "1", Description = "Output stack traces for log messages.")]
        private static readonly ConfigVar ConsoleLogMessagesTraces;

        [ConfigVar(Name = "config.log.level", DefaultValue = "0|1|2", Description = "Sets the log level for the console. 0 = Info, 1 = Warnings, 2 = Errors. Combine using | operator. Ex: config.log.level 0|1")]
        private static readonly ConfigVar ConsoleLogMessagesLevel;
        
        [ConfigVar(Name = "game.maxfps", DefaultValue = "60", Description = "Application.targetFrameRate")]
        private static readonly ConfigVar GameMaxFps;


        public static int PendingCommandsWaitForFrames = 0;
        public static bool PendingCommandsWaitForLoad = false;

        private static List<string> s_pendingCommands = new List<string>();
        private static Dictionary<string, ConsoleCommand> s_commands = new Dictionary<string, ConsoleCommand>();
        private const int K_HistoryCount = 50;
        private static string[] s_history = new string[K_HistoryCount];
        private static int s_historyNextIndex = 0;
        private static int s_historyIndex = 0;

        public delegate void MethodDelegate(string[] args);

        private static IConsoleUI s_consoleUI;
        private static string s_lastMsg = "";
        private static double s_timeLastMsg;

        private static HashSet<string> s_logMessages = new HashSet<string>();
        private static LogType s_consoleLogLevel;

        public static void Init(IConsoleUI consoleUI) {
            s_consoleUI = consoleUI;
            s_consoleUI.Init();


            AddCommand("help", CmdHelp, "Show available commands.");
            AddCommand("vars", CmdVars, "Show available variables.");
            AddCommand("exec", CmdExec, "Executes commands from file.");

            AddCommand("clear", CmdClear, "Clears the console.");

            SetLogLevelParser(ConsoleLogMessagesLevel.Value);

            Application.logMessageReceived += OnLogMessage;

            Write("Console ready");
        }

        public static void Shutdown() {
            s_pendingCommands = new List<string>();
            PendingCommandsWaitForFrames = 0;
            PendingCommandsWaitForLoad = false;
            s_commands = new Dictionary<string, ConsoleCommand>();
            s_history = new string[K_HistoryCount];
            s_logMessages = new HashSet<string>();
            s_historyNextIndex = 0;
            s_historyIndex = 0;

            Application.logMessageReceived -= OnLogMessage;

            s_consoleUI.Shutdown();
        }

        private static void SetLogLevelParser(string input) {
            if (string.IsNullOrWhiteSpace(input)) {
                Write("Input string is empty.");
                return;
            }

            string[] parts = input.Split('|');

            LogType result = default;

            for (int i = 0; i < parts.Length; i++) {
                if (!int.TryParse(parts[i].Trim(), out int value)) {
                    Write($"Invalid input: {input}");
                    return;
                }

                if (!Enum.IsDefined(typeof(LogType), value)) {
                    Write($"Invalid LogType value: {value}");
                    return;
                }

                if (i == 0)
                    result = (LogType)value;
                else
                    result |= (LogType)value;
            }

            s_consoleLogLevel = result;
        }

        private static void OnLogMessage(string msg, string stackTrace, LogType logType) {
            if (ConsoleLogMessages.IntValue == 0 || logType.HasFlag(s_consoleLogLevel) || (ConsoleLogMessagesCollapse.IntValue == 1 && s_logMessages.Contains(stackTrace)))
                return;

            if (stackTrace.Contains("Volt.Utils.Dev"))
                return;

            if (ConsoleLogMessagesCollapse.IntValue == 1)
                s_logMessages.Add(stackTrace);

            Write($"[{Enum.GetName(typeof(LogType), logType)}] {msg}");

            if (ConsoleLogMessagesTraces.IntValue == 1)
                Write(stackTrace);
        }

        static void OutputString(string message) {
            s_consoleUI?.OutputString(message);
        }

        public static void Write(string msg) {
            // Have to condition on cvar being null as this may run before cvar system is initialized
            if (ConsoleShowLastLine != null && ConsoleShowLastLine.IntValue > 0) {
                s_lastMsg = msg;
                s_timeLastMsg = Time.time;
            }
            OutputString(msg);
        }

        public static void AddCommand(string name, MethodDelegate method, string description, int tag = 0) {
            name = name.ToLower();
            if (s_commands.ContainsKey(name)) {
                OutputString("Cannot add command " + name + " twice");
                return;
            }
            s_commands.Add(name, new ConsoleCommand(name, method, description, tag));
        }

        public static bool RemoveCommand(string name) {
            return s_commands.Remove(name.ToLower());
        }

        public static void RemoveCommandsWithTag(int tag) {
            var removals = new List<string>();
            foreach (var c in s_commands) {
                if (c.Value.Tag == tag)
                    removals.Add(c.Key);
            }
            foreach (var c in removals)
                RemoveCommand(c);
        }

        public static void ProcessCommandLineArguments(string[] arguments) {
            // Process arguments that have '+' prefix as console commands. Ignore all other arguments

            OutputString("ProcessCommandLineArguments: " + string.Join(" ", arguments));

            var commands = new List<string>();

            foreach (var argument in arguments) {
                var newCommandStarting = argument.StartsWith("+") || argument.StartsWith("-");

                // Skip leading arguments before we have seen '-' or '+'
                if (commands.Count == 0 && !newCommandStarting)
                    continue;

                if (newCommandStarting)
                    commands.Add(argument);
                else
                    commands[^1] += " " + argument;
            }

            foreach (var command in commands) {
                if (command.StartsWith("+"))
                    EnqueueCommandNoHistory(command[1..]);
            }
        }

        public static bool IsOpen() {
            return s_consoleUI.IsOpen();
        }

        public static void SetOpen(bool open) {
            s_consoleUI.SetOpen(open);
        }

        public static void SetPrompt(string prompt) {
            s_consoleUI.SetPrompt(prompt);
        }

        public static void ConsoleUpdate() {
            s_consoleUI.ConsoleUpdate();

            if (ConsoleLogMessagesLevel.ChangeCheck()) {
                SetLogLevelParser(ConsoleLogMessagesLevel.Value);
            }

            if (GameMaxFps.ChangeCheck()) {
                Application.targetFrameRate = GameMaxFps.IntValue;
            }

            while (s_pendingCommands.Count > 0) {
                if (PendingCommandsWaitForFrames > 0) {
                    PendingCommandsWaitForFrames--;
                    break;
                }
                if (PendingCommandsWaitForLoad) {
                    //if (!Game.game.levelManager.IsCurrentLevelLoaded())
                    //    break;
                    PendingCommandsWaitForLoad = false;
                }
                // Remove before executing as we may hit an 'exec' command that wants to insert commands
                var cmd = s_pendingCommands[0];
                s_pendingCommands.RemoveAt(0);
                ExecuteCommand(cmd);
            }
        }

        public static void ConsoleLateUpdate() {
            s_consoleUI.ConsoleLateUpdate();
        }

        public static void ExecuteCommand(string command) {
            var tokens = Tokenize(command);
            if (tokens.Count < 1)
                return;

            OutputString('>' + command.AddColor(Color.yellow));
            var commandName = tokens[0].ToLower();


            if (s_commands.TryGetValue(commandName, out ConsoleCommand consoleCommand)) {
                var arguments = tokens.GetRange(1, tokens.Count - 1).ToArray();
                consoleCommand.Method(arguments);
            } else if (ConfigVar.ConfigVars.TryGetValue(commandName, out ConfigVar configVar)) {
                if (tokens.Count == 2) {
                    configVar.Value = tokens[1];
                    if (ConsoleAutoSave != null && ConsoleAutoSave.IntValue > 0)
                        ConfigVar.SaveChangedVars();
                } else if (tokens.Count == 1) {
                    // Print value
                    OutputString(string.Format("{0} = {1}", configVar.Name, configVar.Value));
                } else {
                    OutputString("Too many arguments");
                }
            } else {
                OutputString("Unknown command: " + tokens[0]);
            }
        }

        public static void EnqueueCommandNoHistory(string command) {
            Debug.Log(command);
            s_pendingCommands.Add(command);
        }

        public static void EnqueueCommand(string command) {
            s_history[s_historyNextIndex % K_HistoryCount] = command;
            s_historyNextIndex++;
            s_historyIndex = s_historyNextIndex;

            EnqueueCommandNoHistory(command);
        }


        public static string TabComplete(string prefix) {
            // Look for possible tab completions
            List<string> matches = new List<string>();

            foreach (var c in s_commands) {
                var name = c.Key;
                if (!name.StartsWith(prefix, true, null))
                    continue;
                matches.Add(name);
            }

            foreach (var v in ConfigVar.ConfigVars) {
                var name = v.Key;
                if (!name.StartsWith(prefix, true, null))
                    continue;
                matches.Add(name);
            }

            if (matches.Count == 0)
                return prefix;

            // Look for longest common prefix
            int lcp = matches[0].Length;
            for (var i = 0; i < matches.Count - 1; i++) {
                lcp = Mathf.Min(lcp, CommonPrefix(matches[i], matches[i + 1]));
            }
            prefix += matches[0][prefix.Length..lcp];
            if (matches.Count > 1) {
                // write list of possible completions
                for (var i = 0; i < matches.Count; i++)
                    Console.Write(" " + matches[i]);
            } else {
                prefix += " ";
            }
            return prefix;
        }

        public static string HistoryUp(string current) {
            if (s_historyIndex == 0 || s_historyNextIndex - s_historyIndex >= K_HistoryCount - 1)
                return "";

            if (s_historyIndex == s_historyNextIndex) {
                s_history[s_historyIndex % K_HistoryCount] = current;
            }

            s_historyIndex--;

            return s_history[s_historyIndex % K_HistoryCount];
        }

        public static string HistoryDown() {
            if (s_historyIndex == s_historyNextIndex)
                return "";

            s_historyIndex++;

            return s_history[s_historyIndex % K_HistoryCount];
        }

        private static void CmdHelp(string[] arguments) {
            OutputString("Available commands:");

            var lines = new List<string[]>();

            foreach (var c in s_commands)
                lines.Add(new[] { c.Value.Name, c.Value.Description });

            OutputString(ConsoleUtility.PadElementsInLines(lines, 3));
        }

        private static void CmdVars(string[] arguments) {
            var varNames = new List<string>(ConfigVar.ConfigVars.Keys);
            varNames.Sort();

            foreach (var v in varNames) {
                var cv = ConfigVar.ConfigVars[v];
                OutputString(string.Format("{0} = {1}", cv.Name, cv.Value));
            }
        }

        private static void CmdExec(string[] arguments) {
            bool silent = false;
            string filename;
            if (arguments.Length == 1) {
                filename = arguments[0];
            } else if (arguments.Length == 2 && arguments[0] == "-s") {
                silent = true;
                filename = arguments[1];
            } else {
                OutputString("Usage: exec [-s] <filename>");
                return;
            }

            try {
                var lines = System.IO.File.ReadAllLines(filename);
                s_pendingCommands.InsertRange(0, lines);
                if (s_pendingCommands.Count > 128) {
                    s_pendingCommands.Clear();
                    OutputString("Command overflow. Flushing pending commands!!!");
                }
            } catch (Exception e) {
                if (!silent)
                    OutputString("Exec failed: " + e.Message);
            }
        }

        private static void CmdClear(string[] arguments) {
            s_consoleUI?.Clear();
        }

        // Returns length of largest common prefix of two strings
        private static int CommonPrefix(string a, string b) {
            int minl = Mathf.Min(a.Length, b.Length);
            for (int i = 1; i <= minl; i++) {
                if (!a.StartsWith(b[..i], true, null))
                    return i - 1;
            }
            return minl;
        }

        private static void SkipWhite(string input, ref int pos) {
            while (pos < input.Length && " \t".IndexOf(input[pos]) > -1) {
                pos++;
            }
        }

        private static string ParseQuoted(string input, ref int pos) {
            pos++;
            int startPos = pos;
            while (pos < input.Length) {
                if (input[pos] == '"' && input[pos - 1] != '\\') {
                    pos++;
                    return input.Substring(startPos, pos - startPos - 1);
                }
                pos++;
            }
            return input[startPos..];
        }

        private static string Parse(string input, ref int pos) {
            int startPos = pos;
            while (pos < input.Length) {
                if (" \t".IndexOf(input[pos]) > -1) {
                    return input[startPos..pos];
                }
                pos++;
            }
            return input[startPos..];
        }

        private static List<string> Tokenize(string input) {
            var pos = 0;
            var res = new List<string>();
            var c = 0;
            while (pos < input.Length && c++ < 10000) {
                SkipWhite(input, ref pos);
                if (pos == input.Length)
                    break;

                if (input[pos] == '"' && (pos == 0 || input[pos - 1] != '\\')) {
                    res.Add(ParseQuoted(input, ref pos));
                } else
                    res.Add(Parse(input, ref pos));
            }
            return res;
        }
    }
}