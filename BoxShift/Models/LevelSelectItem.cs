namespace BoxShift.Models;

public class LevelSelectItem
{
    public int LevelIndex { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public int BestMoves { get; set; }

    public string ProgressText
    {
        get
        {
            if (IsCompleted)
            {
                return $"Completed | Best: {BestMoves} moves";
            }

            return "Not completed";
        }
    }
}