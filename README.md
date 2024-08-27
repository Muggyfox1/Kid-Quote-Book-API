# Kid Quote Book API
This is an API developed with ASP.net for the mobile app "Kid Quote Book." It provides a way to back up and store your data from the app without having to manually handle the files.

# Requirements 📝
1. Knowing how to port forward.
2. ASP.NET 8 (it's the current LTS version): https://dotnet.microsoft.com/en-us/download/dotnet/8.0

# Installation and Set up 🛠️
1. Download and extract the release files.
2. In appsettings.json
  a. on line 8, change the IP to your hosting PC local IP
3. Port forward the local IP in your routers settings
4. run Kids Quote Book API.exe
5. navigate to *your host PC IP*/kqb to confirm it's running.
  a. it should say "hello world!"

# Configuration ⚙️
You can configure some settings in appsettings.json.

MaxFileMB: determines the max size quote book data files can be. The default is 10, which has been more than enough for testing uses.

# Usage 👟
## TODO
explain it requires the app to be used. 
and how to set it up in the app.
and how to make sure it works on the host machine.

# Data Directory 📂
This API puts its data in the "%appdata%\Local\Kids Quote Book API" directory.

log.csv: logs information and errors that occur from bad API calls or in the code.

"random" files: These are the data files for quotebooks that get saved to the API. They're saved in a json format, but I've left the extension off of them.

# Contribution 💻
The best way to contribute is to try this out and give feedback on what could be done better.
