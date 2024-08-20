using Kids_Quote_Book_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Kids_Quote_Book_API.Controllers {
    public partial class BookController : ControllerBase {
        private readonly ILogger<BookController> _logger;

        private readonly IHandleBookData _dataHandler;

        private readonly int _maxFileMB;

        public BookController(ILogger<BookController> logger, IHandleBookData dataHandler, IConfiguration configuration) {
            _logger = logger;
            _dataHandler = dataHandler;

            int MbSize = 1;
            if(configuration != null ) {
                MbSize = int.Parse(configuration["MaxFileMB"] ?? "1");
            }
            _maxFileMB = MbSize * 1024 * 1024;
        }

        private IActionResult AttemptBookUpload(API_PostBookModel bookPostModel) {
            if(IsInvalidData(bookPostModel)) {
                _logger.LogWarning("New Book Upload; Invalid request, incomplete request;");
                return BadRequest("Invalid request, incomplete data");
            }

            string hashedPassword = HashPassword(bookPostModel.hashedPassword);

            API_BookDataModel BookDataModel = new() {
                EditorToken = bookPostModel.EditorToken,
                hashedPassword = hashedPassword,
                EncryptedJsonBookData = bookPostModel.EncryptedJsonBookData,
                ContactEmail = bookPostModel.ContactEmail,
                LastUpdated = DateTimeOffset.Now,
            };


            string BookDataJson = JsonConvert.SerializeObject(BookDataModel);

            if(!IsDataSizeAcceptable(BookDataJson)) {
                _logger.LogWarning("New Book Upload; Data size exceeds the allowed limit (10MB)");
                return BadRequest("Data size exceeds the allowed limit (10MB)");
            }

            string token = _dataHandler.SaveBook(BookDataModel, null);

            _logger.LogInformation($"New book upload; {token}");
            return Ok(token);
        }

        private IActionResult CheckForBookUpdate(API_PostBookModel readBookModel) {
            var validationError = ValidateRequestAndCheckPassword(readBookModel, out API_BookDataModel bookDataModel, false);
            if(validationError != null) {
                return validationError;
            }

            _logger.LogInformation($"Book checked for update; {readBookModel.Token}");
            bool needsUpdated = readBookModel.LastUpdated < bookDataModel.LastUpdated;
            return Ok(needsUpdated);
        }

        private IActionResult AttemptGetBookByData(API_PostBookModel readBookModel) {
            var validationError = ValidateRequestAndCheckPassword(readBookModel, out API_BookDataModel bookDataModel, false);
            if(validationError != null) {
                return validationError;
            }
            var encryptedBookData = bookDataModel.EncryptedJsonBookData;
            _logger.LogInformation($"Book read; {readBookModel.Token}");
            return Ok(encryptedBookData);
        }

        private IActionResult AttemptBookUpdate(API_PostBookModel updatedBookPutModel) {
            var validationError = ValidateRequestAndCheckPassword(updatedBookPutModel, out API_BookDataModel bookDataModel, true);
            if(validationError != null) {
                return validationError;
            }

            bookDataModel.EncryptedJsonBookData = updatedBookPutModel.EncryptedJsonBookData;
            bookDataModel.ContactEmail = updatedBookPutModel.ContactEmail;
            bookDataModel.LastUpdated = updatedBookPutModel.LastUpdated;
            string bookDataJson = JsonConvert.SerializeObject(bookDataModel);
            if(!IsDataSizeAcceptable(bookDataJson)) {
                _logger.LogError($"Data size exceeds the allowed limit (10MB); {updatedBookPutModel.Token}"
                );
                return BadRequest("Data size exceeds the allowed limit (10MB)");
            }

            _dataHandler.SaveBook(bookDataModel, updatedBookPutModel.Token);
            _logger.LogInformation($"Book updated; {updatedBookPutModel.Token}");
            return Ok("Book updated");
        }

        private IActionResult AttemptBookDelete(API_PostBookModel bookDeleteModel) {
            var validationError = ValidateRequestAndCheckPassword(bookDeleteModel, out _, true);
            if(validationError != null) {
                return validationError;
            }

            _dataHandler.DeleteBook(bookDeleteModel.Token);

            // TODO: maintain token? but status it as deleted in DB?
            _logger.LogWarning($"BOOK DELETED!!!; {bookDeleteModel.Token}");
            return Ok("Deleted Data successfully");
        }

        private static string HashPassword(string password) {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = SHA256.HashData(passwordBytes);
            string hashResult = BitConverter.ToString(hashedBytes).Replace("-", "");
            return hashResult;
        }

        private bool IsDataSizeAcceptable(string bookDataJson) {
            byte[] jsonDataBytes = Encoding.UTF8.GetBytes(bookDataJson);
            return jsonDataBytes.Length <= _maxFileMB;
        }

        private IActionResult? ValidateRequestAndCheckPassword(API_PostBookModel api_bookModel, out API_BookDataModel bookDataModel, bool editsBook) {
            bookDataModel = new();

            //make sure Data is complete
            if(IsInvalidData(api_bookModel)) {
                _logger.LogWarning("Invalid request, incomplete data;");
                return BadRequest("Invalid request, incomplete data");
            }

            //get book by token
            API_BookDataModel result = new();
            try {
                result = _dataHandler.ReadBook(api_bookModel.Token);
            }
            catch(Exception e) {
                if(e.Message.Contains("NotFound") || e.Message.Contains("not found")) {
                    _logger.LogWarning($"Book not found; {api_bookModel.Token}");
                    return BadRequest("Invalid request, NotFound");
                }
            }

            // reconcile passwords
            string hashedPassword = HashPassword(api_bookModel.hashedPassword);
            api_bookModel.hashedPassword = hashedPassword;
            var doesPasswordMatch = result.hashedPassword == hashedPassword;
            if(!doesPasswordMatch) {
                _logger.LogWarning($"Password doesn't match; {api_bookModel.Token}; expected {result.hashedPassword}; offered {hashedPassword}");
                return BadRequest("Invalid request");
            }

            if(editsBook) {
                if(api_bookModel.EditorToken != result.EditorToken){
                    _logger.LogWarning($"Non-editor tried to edit book; {api_bookModel.Token}");
                    return BadRequest("Invalid request, not an editor");
                }
            }

            bookDataModel = result;
            return null;
        }

        static bool IsInvalidData(API_PostBookModel bookPostModel) {
            return bookPostModel == null
                || string.IsNullOrWhiteSpace(bookPostModel.ContactEmail)
                || string.IsNullOrWhiteSpace(bookPostModel.EncryptedJsonBookData)
                || string.IsNullOrWhiteSpace(bookPostModel.hashedPassword);
        }
    }
}
