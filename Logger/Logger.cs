using System.Runtime.CompilerServices;

public static class Logger
{
    private static readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NoSqlLogs");

    static Logger()
    {
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    public static void SaveLog(string message, [CallerMemberName] string callerName = "")
    {
        string logFilePath = Path.Combine(_logDirectory, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss.fff}.txt");

        try
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message} - Method: {callerName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao gravar log: {ex.Message}");
        }
    }
}
