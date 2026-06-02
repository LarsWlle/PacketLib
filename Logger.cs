using System.Reflection;

namespace PacketLib;

using System;
using System.Diagnostics;

public enum LogLevel {
    Trace,
    Debug,
    Info,
    Warn,
    Error,
    Fatal
}

internal static class Logger {
    private static readonly object Lock = new object();

    // ANSI Colors
    private const string Reset = "\e[0m";
    private const string Gray = "\e[90m";
    private const string Cyan = "\e[36m";
    private const string Green = "\e[32m";
    private const string Yellow = "\e[33m";
    private const string Red = "\e[31m";
    private const string BoldRed = "\e[1;31m";

    public static void Log(LogLevel level, string message) {
        StackFrame frame = new StackFrame(2, false);
        MethodBase? method = frame.GetMethod();

        string source = method?.DeclaringType?.Name ?? "Unknown";

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        (string color, string levelText) = Logger.GetLevelConfig(level);

        lock (Logger.Lock) {
            // Format: [YYYY-MM-dd hour:minute:ms] [LEVEL] [source] message
            Console.WriteLine($"{Logger.Gray}[{timestamp}]{Logger.Reset} {color}[{levelText}]{Logger.Reset} {Logger.Cyan}[{source}]{Logger.Reset} {message}");
        }
    }

    private static (string color, string text) GetLevelConfig(LogLevel level) {
        return level switch {
            LogLevel.Trace => (Logger.Gray, "TRACE"),
            LogLevel.Debug => (Logger.Gray, "DEBUG"),
            LogLevel.Info => (Logger.Green, "INFO "),
            LogLevel.Warn => (Logger.Yellow, "WARN "),
            LogLevel.Error => (Logger.Red, "ERROR"),
            LogLevel.Fatal => (Logger.BoldRed, "FATAL"),
            _ => (Logger.Reset, "LOG  ")
        };
    }

    public static void Trace(string msg) => Logger.Log(LogLevel.Trace, msg);
    public static void Debug(string msg) => Logger.Log(LogLevel.Debug, msg);
    public static void Info(string msg) => Logger.Log(LogLevel.Info, msg);
    public static void Warn(string msg) => Logger.Log(LogLevel.Warn, msg);
    public static void Error(string msg) => Logger.Log(LogLevel.Error, msg);
    public static void Fatal(string msg) => Logger.Log(LogLevel.Fatal, msg);
}