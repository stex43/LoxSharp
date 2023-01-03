namespace Loxsharp;

public static class Lox
{
    private static bool HadError;

    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: loxsharp [script]");
            Environment.Exit(64);
        }

        if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string path)
    {
        var fileText = File.ReadAllText(path);
        Run(fileText);

        if (HadError)
        {
            Environment.Exit(65);
        }
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.Out.Write("> ");
            var line = Console.In.ReadLine();
            
            if (string.IsNullOrWhiteSpace(line))
            {
                break;
            }
            
            Run(line);
            HadError = false;
        }
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.Out.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, string.Empty, message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
        HadError = true;
    }
}