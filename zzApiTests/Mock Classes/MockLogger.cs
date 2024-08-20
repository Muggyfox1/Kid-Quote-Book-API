using Kids_Quote_Book_API.Controllers;
using Microsoft.Extensions.Logging;

namespace zzApiTests {
    internal class MockLogger : ILogger<BookController> {
        public List<MockLogEntry> log = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
            var entry = new MockLogEntry() {
                message = state?.ToString() ?? "state was null :/",
                logLevel = logLevel,
            };

            log.Add(entry);
        }
    }

    internal class MockLogEntry {
        public string message;
        public LogLevel logLevel;
    }
}