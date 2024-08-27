# Kid Quote Book API 📖🛜
This is an API developed with ASP.net for the mobile app "Kid Quote Book." It provides a way to back up and store your data from the app without having to manually handle the files.

# Requirements 📝
1. Knowing how to port forward.
2. ASP.NET 8 (it's the current LTS version): https://dotnet.microsoft.com/en-us/download/dotnet/8.0
3. The Kid Quote Book App

# Installation and Set up 🛠️
1. Download and extract the release files.
2. In appsettings.json
- on line 8, change the IP to your hosting PC local IP
5. Port forward the local IP in your routers settings
6. run Kids Quote Book API.exe
7. navigate to *your host PC IP*/kqb to confirm it's running.
- it should load a page that says "hello world!"

# Configuration ⚙️
You can configure some settings in appsettings.json.

MaxFileMB: determines the max size quote book data files can be. The default is 10, which has been more than enough for testing uses.

# In App usage 📱
### To upload
1. Go to the books page
2. For the book you want to upload, press edit
3. Fill in the require info for uploading
- Email: this can be anything, this is meant for if anyone hosts this API for public use.
- Server Url: this is the IP address the API is hosted on. Should end in /kqb.
- Token: when uploading a book, leave this blank. It will update after uploading.
- Password: used to encrypt the data locally before sending. And is securely sent over to verify your ability access the book.
4. Press Save
5. On the book page you should see the upload book button
6. Press the upload book button

### To upload new book data
- If you are an editor for the book, you can do this
1. Press "Check for Update"
2. If "Download Updated Book" appears, press it
-  it will update that book, adding new data to what you have.
3. Press "Upload Updated Book"
- now all the data you have in that book locally is uploaded.

### To Share
#### On your phone:
1. Go to the books page
2. Press "Export Book to file"
3. In the dialog
- Press the book name to choose what book to export
- if you want to let the person you're sharing this with to also upload new quotes, check the "Allow online editing" checkbox.
4. Press Export
5. A confirmatino dialog appears, letting you know it's been exported.
6. Find the exported file in your phones documents (internal storage/documents/*BookName *Date.json)
7. (Optional) upload it to Google drive or other file sharing service.
8. Send this file to who you want to share it with.

#### On the other persons phone:
1. Download the file
2. Go to the books page
3. Press "Add Book from file"
4. Choose the file you downloaded from their file system
5. on the book list you can now press "Open Book" to read data from the new book.

#### To update from the other persons phone:
1. Press "Check for Update"
- If an update is available you will then have the option to download the update. Otherwise the button will disappear.
2. Press "Download Updated Book"

# Data Directory 📂
This API puts its data in the "%appdata%\Local\Kids Quote Book API" directory.

log.csv: logs information and errors that occur from bad API calls or in the code.

"random" files: These are the data files for quotebooks that get saved to the API. They're saved in a json format, but I've left the extension off of them.

# Contribution 💻
The best way to contribute is to try this out and give feedback on what could be done better.
