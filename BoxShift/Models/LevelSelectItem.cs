namespace BoxShift.Models;

public class LevelSelectItem
{
    public int LevelIndex { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public int BestMoves { get; set; }

    public bool IsCustom { get; set; }

    public string LevelSource
    {
        get
        {
            return IsCustom ? "custom" : "builtin";
        }
    }

    public string ProgressText
    {
        get
        {
            if (IsCustom)
            {
                return "Custom Level";
            }

            if (IsCompleted)
            {
                return $"Completed | Best: {BestMoves} moves";
            }

            return "Not completed";
        }
    }
}