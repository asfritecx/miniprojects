using Microsoft.Extensions.Logging;
using System;

public static class ColorConsole
{
    public static void WriteLine(string message, ConsoleColor color)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previousColor;
    }

    public static void WriteLog(string message, LogLevel logLevel)
    {
        var color = logLevel switch
        {
            LogLevel.Information => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.DarkRed,
            _ => ConsoleColor.White,
        };
        WriteLine(message, color);
    }
}
