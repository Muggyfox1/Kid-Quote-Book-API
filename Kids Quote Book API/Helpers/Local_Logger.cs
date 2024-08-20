using Kids_Quote_Book_API.Controllers;

namespace Kids_Quote_Book_API.Helpers {
    public class Local_Logger : ILogger<BookController> {

        private readonly string DataDirectory;
        private readonly string logFilePath;

        public Local_Logger() {
            DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Kids Quote Book API");
            logFilePath = Path.Combine(DataDirectory, "log.csv");
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
            string message = formatter(state, exception);
            string exceptionMessage = exception?.ToString() ?? String.Empty;
            string logRecord = $"{DateTime.UtcNow}, {logLevel}, {eventId.Id}, \"{message}\", \"{exceptionMessage}\"";

            try {
                if(!File.Exists(logFilePath)) {
                    Directory.CreateDirectory(DataDirectory);
                    string headerLine = "Time, Level, Event, Message, ExceptionMessage\n";
                    File.WriteAllText(logFilePath, headerLine);
                }
                File.AppendAllText(logFilePath, logRecord + Environment.NewLine);
            }
            catch(Exception ex) {
                Console.Error.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}
