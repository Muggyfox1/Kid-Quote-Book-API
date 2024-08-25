using Kids_Quote_Book_API.Helpers;
using Kids_Quote_Book_API.Models;
using Newtonsoft.Json.Linq;

namespace zzApiTests.API_DataHandler_tests {
    public class API_LocalDataHandler_Tests : IDisposable {
        private readonly string _directoryPath;
        private readonly Local_BookDataHandler _handler;

        public API_LocalDataHandler_Tests() {
            // Create a unique directory in the temp directory for each test
            _directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_directoryPath);
            _handler = new Local_BookDataHandler(_directoryPath);
        }

        [Fact]
        public void SaveBook_CreatesNewBook_WhenTokenIsNull() {
            // Arrange
            var bookData = new API_BookDataModel {
                EditorToken = "editor123",
                hashedPassword = "hashedPassword",
                ContactEmail = "test@example.com",
                EncryptedJsonBookData = "encryptedData",
                LastUpdated = DateTimeOffset.Now
            };

            // Act
            var token = _handler.SaveBook(bookData, null);
            var result = _handler.ReadBook(token);

            // Assert
            Assert.NotNull(token);
            Assert.Equal(bookData.EditorToken, result.EditorToken);
            Assert.Equal(bookData.hashedPassword, result.hashedPassword);
            Assert.Equal(bookData.ContactEmail, result.ContactEmail);
            Assert.Equal(bookData.EncryptedJsonBookData, result.EncryptedJsonBookData);
            Assert.Equal(bookData.LastUpdated, result.LastUpdated);
        }

        [Fact]
        public void DeleteBook_RemovesBook_WhenTokenExists() {
            // Arrange
            var bookData = new API_BookDataModel {
                EditorToken = "editor123",
                hashedPassword = "hashedPassword",
                ContactEmail = "test@example.com",
                EncryptedJsonBookData = "encryptedData",
                LastUpdated = DateTimeOffset.Now
            };
            var token = _handler.SaveBook(bookData, null);

            // Act
            _handler.DeleteBook(token);
            
            // Assert
            Assert.Throws<FileNotFoundException>(() => _handler.ReadBook(token));
        }

        [Fact]
        public void ReadBook_ReturnsBook_WhenTokenExists() {
            // Arrange
            var bookData = new API_BookDataModel {
                EditorToken = "editor123",
                hashedPassword = "hashedPassword",
                ContactEmail = "test@example.com",
                EncryptedJsonBookData = "encryptedData",
                LastUpdated = DateTimeOffset.Now
            };
            var token = _handler.SaveBook(bookData, null);

            // Act
            var result = _handler.ReadBook(token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookData.EditorToken, result.EditorToken);
            Assert.Equal(bookData.hashedPassword, result.hashedPassword);
            Assert.Equal(bookData.ContactEmail, result.ContactEmail);
            Assert.Equal(bookData.EncryptedJsonBookData, result.EncryptedJsonBookData);
            Assert.Equal(bookData.LastUpdated, result.LastUpdated);
        }

        [Fact]
        public void ReadBook_ReturnsNull_WhenTokenDoesNotExist() {
            // Arrange
            var nonExistentToken = "nonExistentToken";

            // Act && Assert
            var exception = Assert.Throws<FileNotFoundException>(() => _handler.ReadBook(nonExistentToken));
        }

        [Fact]
        public void SaveBook_UpdatesBook_WhenTokenExists() {
            // Arrange
            var originalBookData = new API_BookDataModel {
                EditorToken = "editor123",
                hashedPassword = "hashedPassword",
                ContactEmail = "test@example.com",
                EncryptedJsonBookData = "encryptedData",
                LastUpdated = DateTimeOffset.Now
            };
            var token = _handler.SaveBook(originalBookData, null);

            var updatedBookData = new API_BookDataModel {
                EditorToken = "editor123-updated",
                hashedPassword = "newHashedPassword",
                ContactEmail = "new@example.com",
                EncryptedJsonBookData = "newEncryptedData",
                LastUpdated = DateTimeOffset.Now.AddDays(1)
            };

            // Act
            _handler.SaveBook(updatedBookData, token);
            var result = _handler.ReadBook(token);

            // Assert
            Assert.Equal(updatedBookData.EditorToken, result.EditorToken);
            Assert.Equal(updatedBookData.hashedPassword, result.hashedPassword);
            Assert.Equal(updatedBookData.ContactEmail, result.ContactEmail);
            Assert.Equal(updatedBookData.EncryptedJsonBookData, result.EncryptedJsonBookData);
            Assert.Equal(updatedBookData.LastUpdated, result.LastUpdated);
        }

        public void Dispose() {
            // Clean up the directory created during the test
            if(Directory.Exists(_directoryPath)) {
                Directory.Delete(_directoryPath, true);
            }
        }
    }

}
