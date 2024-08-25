using Kids_Quote_Book_API.Controllers;
using Kids_Quote_Book_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace zzApiTests {
    public class API_BookController_Tests {
        public API_BookController_Tests() {
            //var dataLimit = 1024 * 1024; //1MB
            mockDataHandler = new MockBookDataHandler();
            mockLogger = new MockLogger();
            _controller = new(mockLogger, mockDataHandler, null);
        }

        MockBookDataHandler mockDataHandler;
        MockLogger mockLogger;
        BookController _controller;

        string data = "this is some data to save that should remain the same.";

        [Fact]
        public void canSave_Data() {
            //arrange
            var apiBookModel = new API_PostBookModel() {
                ContactEmail = "test@email.com",
                EncryptedJsonBookData = data,
                hashedPassword = "password",
            };

            var preActCount = mockDataHandler.bookDatabase.Count;

            //act
            OkObjectResult response = (OkObjectResult)_controller.Post(apiBookModel);

            //assert
            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(response.Value);
            Assert.NotEmpty((string)response.Value);

            Assert.True(mockDataHandler.bookDatabase.Count > preActCount);
            Assert.True(mockDataHandler.bookDatabase.ContainsKey((string)response.Value));
        }

        [Fact]
        public void canRead_Data() {
            //arrange
            var apiBookModel = new API_PostBookModel() {
                ContactEmail = "test@email.com",
                EncryptedJsonBookData = data,
                hashedPassword = "password",
            };

            OkObjectResult response = (OkObjectResult)_controller.Post(apiBookModel);

            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(response.Value);
            Assert.NotEmpty((string)response.Value);

            string token = (string)response.Value;

            //act
            apiBookModel.Token = token;
            response = (OkObjectResult)_controller.GetBookByData(apiBookModel);

            //assert
            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(response.Value);
            Assert.NotEmpty((string)response.Value);
            Assert.Equal(data, response.Value);
        }

        [Fact]
        public void canUpdate_Data() {
            //arrange
            var apiBookModel = new API_PostBookModel() {
                ContactEmail = "test@email.com",
                EncryptedJsonBookData = data,
                hashedPassword = "password",
            };

            OkObjectResult response = (OkObjectResult)_controller.Post(apiBookModel);

            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(response.Value);
            Assert.NotEmpty((string)response.Value);

            string token = (string)response.Value;
            string data2 = new string(data + "more data");
            apiBookModel = new API_PostBookModel() {
                Token = token,
                ContactEmail = "test@email.com",
                EncryptedJsonBookData = data2,
                hashedPassword = "password",
            };

            string preActData = mockDataHandler.bookDatabase[token].EncryptedJsonBookData;

            //act
            response = (OkObjectResult)_controller.Put(apiBookModel);

            //assert
            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(response.Value);
            Assert.NotEmpty((string)response.Value);

            var postActData = mockDataHandler.bookDatabase[token].EncryptedJsonBookData;

            Assert.NotEqual(preActData, postActData);
        }

        [Fact]
        public void canDelete_Data() {
            //arrange
            var apiBookModel = new API_PostBookModel() {
                ContactEmail = "test@email.com",
                EncryptedJsonBookData = data,
                hashedPassword = "password",
            };

            OkObjectResult response1 = (OkObjectResult)_controller.Post(apiBookModel);

            var preActCount = mockDataHandler.bookDatabase.Count;

            string token = (string)response1.Value;
            var apiBookModel2 = new API_PostBookModel() {
                Token = token,
                ContactEmail = "test@email.com",
                EncryptedJsonBookData = data,
                hashedPassword = "password",
            };

            //act
            ObjectResult result = (ObjectResult)_controller.Delete(apiBookModel2);

            //assert
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.NotEmpty((string)result.Value);

            var postActCount = mockDataHandler.bookDatabase.Count;
            Assert.True(postActCount == preActCount - 1);
            Assert.False(mockDataHandler.bookDatabase.ContainsKey(token));
        }

        [Fact]
        public void canCheckForDataUpdate() {

            //arrange
            var apiBookModel = new API_PostBookModel() {
                ContactEmail = "test@email.com",
                EncryptedJsonBookData = data,
                hashedPassword = "password",
                LastUpdated = DateTimeOffset.Now,
            };

            ObjectResult postResult = (ObjectResult)_controller.Post(apiBookModel);
            string token = (string)postResult.Value;

            var apiBookModel2 = new API_PostBookModel() {
                Token = token,
                ContactEmail = "test@email.com",
                EncryptedJsonBookData = data,
                hashedPassword = "password",
                LastUpdated = DateTimeOffset.MinValue,
            };

            //act
            ObjectResult response = (ObjectResult)_controller.CheckForUpdates(apiBookModel2);

            //assert
            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(response.Value);
            Assert.True((bool)response.Value);
        }
    }
}