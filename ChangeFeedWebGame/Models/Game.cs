namespace ChangeFeedWebGame.Models
{
    public class Game
    {
        public string Id { get; set; }
        public int Score { get; set; }
        public int Clicks { get; set; }
        public string UserId { get; set; }
        public List<string> Achievements { get; set; }
    }
}
