using Kids_Quote_Book_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kids_Quote_Book_API.Controllers {
    [ApiController]
    [Route("kqb")]
    public partial class BookController : ControllerBase {

        /// <summary>
        /// Pings the service
        /// <para> Mainly used to see if the service is running or accessible. </para>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get() {
            return "Hello World!";
        }

        [HttpPost]
        public IActionResult Post([FromBody] API_PostBookModel bookPostModel) {
            try {
                IActionResult result = AttemptBookUpload(bookPostModel);
                return result;
            }
            catch(Exception exception) {
                LogException(bookPostModel, exception, "Upload");
                return BadRequest("Invalid Request");
            }
        }

        [HttpPost("checkupdate")]
        public IActionResult CheckForUpdates([FromBody] API_PostBookModel readBookModel) {
            try {
                IActionResult result = CheckForBookUpdate(readBookModel);
                return result;
            }
            catch(Exception exception) {
                LogException(readBookModel, exception, "CheckUpdate");
                return BadRequest("Invalid Request");
            }
        }

        [HttpPost("read")]
        public IActionResult GetBookByData([FromBody] API_PostBookModel readBookModel) {
            try {
                IActionResult result = AttemptGetBookByData(readBookModel);
                return result;
            }
            catch(Exception exception) {
                LogException(readBookModel, exception, "GetBookByData");
                return BadRequest("Invalid Request");
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] API_PostBookModel updatedBookPostModel) {
            try {
                IActionResult result = AttemptBookUpdate(updatedBookPostModel);
                return result;
            }
            catch(Exception exception) {
                LogException(updatedBookPostModel, exception, "CheckUpdate");
                return BadRequest("Invalid Request");
            }

        }

        [HttpDelete]
        public IActionResult Delete([FromBody] API_PostBookModel postBookModel) {
            try {
                IActionResult result = AttemptBookDelete(postBookModel);
                return result;
            }
            catch(Exception exception) {
                LogException(postBookModel, exception, "CheckUpdate");
                return BadRequest("Invalid Request");
            }
        }

        private void LogException(API_PostBookModel bookPostModel, Exception e, string functionName) {
            bookPostModel.hashedPassword = HashPassword(bookPostModel.hashedPassword); //DO NOT SAVE A USERS PASSWORD UNHASHED!
            bookPostModel.EncryptedJsonBookData = "Left blank on purpose.";
            string requestDataJson = JsonConvert.SerializeObject(bookPostModel, Formatting.None);
            string message = $"{functionName} Error; {e.Message}; {requestDataJson}";
            _logger.LogError(message);
        }
    }
}