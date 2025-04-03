namespace _Game.__Scripts.System.DI.Test
{
    public class ScoreManager
    {
        [Inject] 
        public GameService GameService { get; set; }

        public void AddScore(int points)
        {
            GameService.LogMessage($"Added {points} points!");
        }
    }
}