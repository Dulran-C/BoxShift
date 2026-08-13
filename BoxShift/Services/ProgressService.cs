using System.Text.Json;
using BoxShift.Models;

namespace BoxShift.Services;

public class ProgressService
{
    private readonly string _filePath;

    public ProgressService()
    {
        _filePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "progress.json");
    }

    public async Task<List<LevelProgress>> LoadProgressAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<LevelProgress>();
        }

        string json =
            await File.ReadAllTextAsync(_filePath);

        List<LevelProgress>? progress =
            JsonSerializer.Deserialize<List<LevelProgress>>(json);

        return progress ?? new List<LevelProgress>();
    }

    public async Task SaveProgressAsync(
        List<LevelProgress> progress)
    {
        string json =
            JsonSerializer.Serialize(
                progress,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        await File.WriteAllTextAsync(
            _filePath,
            json);
    }

    public async Task RecordCompletionAsync(
        int levelIndex,
        int moveCount)
    {
        List<LevelProgress> progress =
            await LoadProgressAsync();

        LevelProgress? existingProgress =
            progress.FirstOrDefault(
                item => item.LevelIndex == levelIndex);

        if (existingProgress == null)
        {
            existingProgress = new LevelProgress
            {
                LevelIndex = levelIndex,
                IsCompleted = true,
                BestMoves = moveCount
            };

            progress.Add(existingProgress);
        }
        else
        {
            existingProgress.IsCompleted = true;

            if (existingProgress.BestMoves == 0 ||
                moveCount < existingProgress.BestMoves)
            {
                existingProgress.BestMoves =
                    moveCount;
            }
        }

        await SaveProgressAsync(progress);
    }
}