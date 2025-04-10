using Azure.Identity;
using System.Text.Json;
using ChangeFeedWebGame.Models;
using Microsoft.Azure.Cosmos;

namespace ChangeFeedWebGame
{
    public class GameService
    {
        private Container games;
        
        public GameService(string endpoint, string database, string container) 
        {
            CosmosClientOptions clientOptions = new CosmosClientOptions()
            {
                UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
            };
            CosmosClient client = new CosmosClient(endpoint, new DefaultAzureCredential(), clientOptions);
            Database db = client.GetDatabase(database);
            games = db.GetContainer(container);
        }

        public async Task CreateGame(Game game)
        {
            await games.CreateItemAsync(game, new PartitionKey(game.UserId));
        }

        public async Task<Game> GetGame(string gameId, string userId)
        {
            return await games.ReadItemAsync<Game>(gameId, new PartitionKey(userId));
        }

        public async Task DeleteGame(string gameId, string userId)
        {
            await games.DeleteItemAsync<Game>(gameId, new PartitionKey(userId));
        }

        public async Task<Game> GetMostRecentGame(string userId)
        {
            string queryText = $"""
                SELECT TOP 1 *
                FROM c 
                WHERE c.userId = @user 
                ORDER BY c._ts DESC
                """;
            QueryDefinition query = new QueryDefinition(queryText)
                .WithParameter("@user", userId);

            FeedIterator<Game> response = games.GetItemQueryIterator<Game>(
                query, 
                null,
                new QueryRequestOptions() { PartitionKey = new PartitionKey(userId) });

            List<Game> output = new();
            while (response.HasMoreResults)
            {
                FeedResponse<Game> results = await response.ReadNextAsync();
                output.AddRange(results);
            }

            return output.FirstOrDefault();
        }

        public async Task<List<Game>> GetAllGames(string userId)
        {
            string queryText = $"""
                SELECT *
                FROM c 
                WHERE c.userId = @user
                ORDER BY c._ts DESC
                """;
            QueryDefinition query = new QueryDefinition(queryText)
                .WithParameter("@user", userId);

            FeedIterator<Game> response = games.GetItemQueryIterator<Game>(
                query,
                null,
                new QueryRequestOptions() { PartitionKey = new PartitionKey(userId) });

            List<Game> output = new();
            while (response.HasMoreResults)
            {
                FeedResponse<Game> results = await response.ReadNextAsync();
                output.AddRange(results);
            }

            return output;
        }

        public async Task<int> UpdateScore(Game game, int incrementScore, string achievement)
        {
            List<PatchOperation> operations = new List<PatchOperation>() 
            { 
                PatchOperation.Increment("/score", incrementScore),
                PatchOperation.Increment("/clicks", 1),
            };
            if (achievement != "")
            {
                operations.Add(PatchOperation.Add("/achievements/0",  achievement));
            }
            Game updatedGame = await games.PatchItemAsync<Game>(game.Id, new PartitionKey(game.UserId), operations);

            return updatedGame.Score;
        }
    }
}
