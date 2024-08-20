using Kids_Quote_Book_API.Controllers;
using Kids_Quote_Book_API.Models;

namespace zzApiTests {
    internal class MockBookDataHandler : IHandleBookData {

        public MockBookDataHandler() {
            bookDatabase = new Dictionary<string, API_BookDataModel>();
        }

        public string SaveBook(API_BookDataModel googleBookDataModel, string? token) {
            if(token == null) {
                token = Guid.NewGuid().ToString();
                bookDatabase[token] = googleBookDataModel;
            }
            else {
                bookDatabase[token] = googleBookDataModel;
            }
            return token;
        }

        public readonly Dictionary<string, API_BookDataModel> bookDatabase;

        public void DeleteBook(string token) {
            if(!bookDatabase.ContainsKey(token)) {
                throw new KeyNotFoundException($"Book with token '{token}' NotFound.");
            }
            bookDatabase.Remove(token);
        }

        public API_BookDataModel ReadBook(string token) {
            if(!bookDatabase.ContainsKey(token)) {
                throw new KeyNotFoundException($"Book with token '{token}' NotFound.");
            }
            return bookDatabase[token];
        }
    }
}