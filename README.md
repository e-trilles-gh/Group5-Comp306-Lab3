1. Edit the <appsettings.json> file. Use the code below, and provide the necessary
  information according to your system. Change the <SERVER_NAME> which is the "Server name"
  from your Microsoft SQL Server Management Studio. :

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "connectServer": "data source=<SERVER_NAME>; initial catalog=Lab3DB; integrated security=True; TrustServerCertificate=True"
  }
}
