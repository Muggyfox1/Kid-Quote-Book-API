namespace Kids_Quote_Book_API.Models
{
    public class API_BookDataModel
    {
        public string EditorToken { get; set; } = "x";
        public string hashedPassword { get; set; } = "";
        public string ContactEmail { get; set; } = "";
        public string EncryptedJsonBookData { get; set; } = "";
        public DateTimeOffset LastUpdated { get; set; }
    }
}