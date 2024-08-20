using Kids_Quote_Book_API.Controllers;
using Kids_Quote_Book_API.Models;
using Newtonsoft.Json;

namespace Kids_Quote_Book_API.Helpers {
    public class Local_BookDataHandler : IHandleBookData {
        private readonly string DataDirectory;

        public Local_BookDataHandler() {
            DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Kids Quote Book API");
        }

        public string SaveBook(API_BookDataModel BookDataModel, string? token) {
            string dataToSave = JsonConvert.SerializeObject(BookDataModel);
            string fileName = token ?? NewFileName();
            string filePath = Path.Combine(DataDirectory, fileName);
            File.WriteAllText(filePath, dataToSave);
            return fileName;
        }

        public API_BookDataModel ReadBook(string token) {
            string filePath = Path.Combine(DataDirectory, token);
            if(File.Exists(filePath)) {
                string fileContent = File.ReadAllText(filePath);
                API_BookDataModel? bookData = JsonConvert.DeserializeObject<API_BookDataModel>(fileContent);

                if(bookData != null) {
                    return bookData;
                }
                else {
                    throw new Exception($"An error occured when reading book data: {token}");
                }
            }
            else {
                throw new FileNotFoundException($"Book not found: {token}");
            }
        }

        public void DeleteBook(string token) {
            string filePath = Path.Combine(DataDirectory, token);
            File.Delete(filePath);
        }

        private string NewFileName() {
            var newName = Guid.NewGuid().ToString();
            string namePath = Path.Combine(DataDirectory, newName);
            var fileExists = File.Exists(namePath);

            if(fileExists) {
                return NewFileName();
            }
            else {
                return newName;
            }
        }
    }
}
