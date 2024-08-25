using Kids_Quote_Book_API.Helpers;
using Microsoft.Extensions.Logging;

namespace zzApiTests.API_Logger_Tests {
    public class API_LocalLogger_Tests : IDisposable {
        private readonly string _testDirectory;
        private readonly string _testLogFilePath;
        private readonly Local_Logger _logger;

        public API_LocalLogger_Tests() {
            // Create a unique test directory and log file path
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _testLogFilePath = Path.Combine(_testDirectory, "log.csv");

            // Initialize the Local_Logger with the test directory
            _logger = new Local_Logger(_testDirectory);
        }

        [Fact]
        public void Log_CreatesLogFile_WhenFileDoesNotExist() {
            // Act
            _logger.Log(LogLevel.Information, new EventId(1), "Test message", null, (state, exception) => state);

            // Assert
            Assert.True(File.Exists(_testLogFilePath), "Log file should be created.");
        }

        [Fact]
        public void Log_WritesCorrectLogEntry() {
            // Arrange
            var logMessage = "Test message";
            var logLevel = LogLevel.Information;
            var eventId = new EventId(1);

            // Act
            _logger.Log(logLevel, eventId, logMessage, null, (state, exception) => state);

            // Assert
            var lines = File.ReadAllLines(_testLogFilePath);
            var lastLine = lines[^1]; // Last line should be the most recent log entry

            Assert.Contains(logMessage, lastLine);
            Assert.Contains(logLevel.ToString(), lastLine);
            Assert.Contains(eventId.Id.ToString(), lastLine);
        }

        [Fact]
        public void Log_CreatesLogFileWithHeader_WhenFileDoesNotExist() {
            // Act
            _logger.Log(LogLevel.Information, new EventId(1), "Test message", null, (state, exception) => state);

            // Assert
            var headerLine = "Time, Level, Event, Message, ExceptionMessage";
            var firstLine = File.ReadLines(_testLogFilePath).First();

            Assert.Equal(headerLine, firstLine);
        }

        [Fact]
        public void Log_HandlesDirectoryCreation() {
            // Arrange
            var newLogDirectory = Path.Combine(Path.GetTempPath(), "TestDirectory");
            var testLogFilePath = Path.Combine(newLogDirectory, "log.csv");

            // Use a new Local_Logger instance with a different directory
            var logger = new Local_Logger(newLogDirectory);

            // Act
            logger.Log(LogLevel.Information, new EventId(1), "Test message", null, (state, exception) => state);

            // Assert
            Assert.True(Directory.Exists(newLogDirectory), "Log directory should be created.");
            Assert.True(File.Exists(testLogFilePath), "Log file should be created.");

            //Clean Up
            if(Directory.Exists(newLogDirectory)) {
                Directory.Delete(newLogDirectory, true);
            }
        }

        public void Dispose() {
            // Clean up the test directory and log file after each test
            if(Directory.Exists(_testDirectory)) {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
