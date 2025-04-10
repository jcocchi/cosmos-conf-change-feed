# Azure Cosmos DB Change Feed Demo

This project contains two applications. Both are intended to be run simultaneously to illustrate how [all versions and deletes change feed mode](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/change-feed-modes?tabs=all-versions-and-deletes) helps you process changes to your data.
 - Blazor web app simulating a game with updates to the underlying data in Azure Cosmos DB
 - Python app to process change feed as users play the game 

## Requirements

- An Azure Cosmos DB account. If you don't have one you can [try Azure Cosmos DB for free](https://learn.microsoft.com/en-us/azure/cosmos-db/try-free?tabs=nosql).
    - This application uses Microsoft Entra ID for authentication instead of relying on primary keys. [Learn about configuring data plane role-based access control.](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/security/how-to-grant-data-plane-role-based-access?tabs=built-in-definition%2Ccsharp&pivots=azure-interface-cli)
- [Dotnet 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Python](https://www.python.org/downloads/)

## Setup and Run

### Web app

Edit the `CosmosDB` section in the `ChangeFeedWebGame/appsettings.json` file with your account endpoint.

```json
  "CosmosDb": {
    "Endpoint": "<Your Cosmos DB Endpoint>",
    "Database": "GameDB",
    "Container": "games"
  }
```

Open the solution file in Visual Studio to run the application. Alternately, you can run the project from the command line.

```cmd
cd ChangeFeedWebGame
dotnet run
```

### Change feed app

Edit the `config.py` file with your account endpoint.

```python
settings = {
    'endpoint': os.environ.get('ACCOUNT_ENDPOINT', '<Your Cosmos DB Endpoint>'),
    'database_id': os.environ.get('DATABASE_ID', 'GameDB'),
    'container_id': os.environ.get('CONTAINER_ID', 'games'),
}
```

Before running the project, install the required packages. All versions and deletes change feed mode is available in the preview release of the `azure-cosmos` package. After installing the packages, run the application.

```cmd
cd change-feed

pip install azure-cosmos==4.10.0b4
pip install azure-identity

python read_change_feed.py
```
