using Kids_Quote_Book_API.Controllers;
using Kids_Quote_Book_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace zzApiTests.API_Controller_Tests {
    public class API_BookController_ErrorTests {
        long dataLimit = 1024 * 1024; //1mb
        MockBookDataHandler mockDataHandler;
        MockLogger mockLogger;
        BookController _controller;

        const string data = "this is some data to save that should remain the same.";

        public API_BookController_ErrorTests() {
            mockDataHandler = new MockBookDataHandler();
            mockLogger = new MockLogger();
            _controller = new(mockLogger, mockDataHandler, null);
        }

        [Theory]
        [InlineData(" ", "password", data)]
        [InlineData("test@email.com", " ", data)]
        [InlineData("test@email.com", "password", " ")]
        public void canHandle_IncompleteDataOnUpload(string email, string password, string data) {
            //arrange
            API_PostBookModel api_bookModel = new() {
                ContactEmail = email,
                hashedPassword = password,
                EncryptedJsonBookData = data
            };

            //act
            var response = (ObjectResult)_controller.Post(api_bookModel);

            //assert
            Assert.NotNull(response);
            Assert.Equal(400, response.StatusCode);
            Assert.NotEmpty((string)response.Value);

            //check logger as well
            Assert.NotEmpty(mockLogger.log);

            var hasCorrectLogMessage = mockLogger.log.Any(l => {
                return l.message.Contains("incomplete request");
            });
            Assert.True(hasCorrectLogMessage);

            //check Datahandler for emptiness as well
            Assert.Empty(mockDataHandler.bookDatabase);
        }

        [Theory]
        [InlineData("test@email.com", "password", data)]
        [InlineData("test@email.com", "1", data)]
        public void canHandle_IncorrectPassword(string email, string password, string data) {
            //arrange
            API_PostBookModel api_bookModel = new() {
                ContactEmail = email,
                hashedPassword = password,
                EncryptedJsonBookData = data
            };

            var response = (ObjectResult)_controller.Post(api_bookModel);

            string token = (string)response.Value;

            api_bookModel = new() {
                Token = token,
                ContactEmail = email,
                hashedPassword = password + "1",
                EncryptedJsonBookData = data
            };

            //act
            var response2 = (ObjectResult)_controller.Put(api_bookModel);

            Assert.NotNull(response2);
            Assert.Equal(400, response2.StatusCode);
            Assert.NotEmpty((string)response2.Value);

            //check logger as well
            Assert.NotEmpty(mockLogger.log);

            var hasCorrectLogMessage = mockLogger.log.Any(l => {
                return l.message.Contains("Password doesn't match");
            });
            Assert.True(hasCorrectLogMessage);
        }

        [Theory]
        [InlineData("test@email.com", "password")]
        public void canHandle_ExceedingMaxData(string email, string password) {
            //arrange
            char[] dataChars = new char[dataLimit + 1];

            API_PostBookModel api_bookModel = new() {
                ContactEmail = email,
                hashedPassword = password,
                EncryptedJsonBookData = new string(dataChars)
            };

            //act
            var response = (ObjectResult)_controller.Post(api_bookModel);

            //assert
            Assert.NotNull(response);
            Assert.Equal(400, response.StatusCode);
            Assert.NotEmpty((string)response.Value);

            //check logger as well
            Assert.NotEmpty(mockLogger.log);

            var hasCorrectLogMessage = mockLogger.log.Any(l => {
                return l.message.Contains("Data size exceeds the allowed limit");
            });
            Assert.True(hasCorrectLogMessage);

            //check Datahandler for emptiness as well
            Assert.Empty(mockDataHandler.bookDatabase);
        }

        [Theory]
        [InlineData("test@email.com", "password", data)]
        public void canHandle_InvalidToken(string email, string password, string data) {
            //arrange
            API_PostBookModel api_bookModel = new() {
                ContactEmail = email,
                hashedPassword = password,
                EncryptedJsonBookData = data
            };

            var response = (ObjectResult)_controller.Put(api_bookModel);

            string token = (string)response.Value;

            api_bookModel = new() {
                Token = "invalid token",
                ContactEmail = email,
                hashedPassword = password + "1",
                EncryptedJsonBookData = data
            };

            //act
            var response2 = (ObjectResult)_controller.Put(api_bookModel);

            Assert.NotNull(response2);
            Assert.Equal(400, response2.StatusCode);
            Assert.NotEmpty((string)response2.Value);

            //check logger as well
            Assert.NotEmpty(mockLogger.log);

            var hasCorrectLogMessage = mockLogger.log.Any(l => {
                return l.message.Contains("Book not found");
            });
            Assert.True(hasCorrectLogMessage);
        }
    }
}
