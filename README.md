#### Description

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
There aren't any src/Presentation/appsettings.json files included in this repository. Create these files manually with following content.

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


#### Database

- For reference to generate a database see _src/Infrastructure/Extension/Passport/...Extension.cs_.
- Use interface _src/Application/Interface/DataAccess/IDataAccess_ for implementing any relational database and mark an inherited class with the interface IPassportDataAccess (see _src/Infrastructure/DataAccess/PassportDataAccess.cs_).

**Note:** Each passport can be enabled by an existing authority. Initial this authorized passport must be created manually in the database.