namespace ChangeFeedWebGame.Options
{
    public class CosmosDb
    {
        public required string Endpoint { get; init; }
        
        public required string Database { get; init; }

        public required string Container { get; init; }
    }
}
