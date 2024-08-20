using Kids_Quote_Book_API.Models;

namespace Kids_Quote_Book_API.Controllers {
    public interface IHandleBookData {
        public abstract void DeleteBook(string token);
        public abstract API_BookDataModel ReadBook(string token);
        public abstract string SaveBook(API_BookDataModel BookDataModel, string? token);
    }
}