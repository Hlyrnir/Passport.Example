This repository shows an implementation of the idea to use a passport for authorization.

- Passport: A passport has to be enabled and valid to generate a JWT token.
- PassportHolder: A passport is issued by an authority to a specific holder. This class contains all information about this holder.
- PassportVisa: For every endpoint you can specify a necessary visa. This has to be granted by an authority.
- PassportToken: Credentials at a specific provider will generate a token. This token is needed for generating a JWT token.

For realisation following concepts has been used.

- Clean architecture
- Domain driven design
- Message pipeline (Request -> Authorization -> Validation -> Handler -> Result -> Response) using Mediator
- ORM using Dapper

**Important!**
There aren't any Presentation/appsettings.json files included in this repository. Create these files manually with following content.

    {
      "AllowedHosts": "*",
      "ConnectionStrings": {
        "Default": "Data Source=[DATABASE_NAME].db; Mode=ReadWrite",
        "Passport": "Data Source=[DATABASE_NAME].db; Mode=ReadWrite",
        "PhysicalData": "Data Source=[DATABASE_NAME].db; Mode=ReadWrite"
      },
      "DataProtection": {
        "KeyStoragePath": "[PATH]",
        "EncryptionKey": "[KEY]"
      },
      "SignatureHash": {
        "PublicKey": "[KEY]"
      },
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Information",
          "Microsoft.AspNetCore.DataProtection": "Information"
        }
      },
      "JwtSetting": {
        "SecretKey": "[KEY]",
        "Issuer": "https://localhost:[PORT]",
        "Audience": "https://localhost:[PORT]"
      }
    }
