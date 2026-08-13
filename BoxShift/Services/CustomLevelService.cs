using System.Text.Json;
using BoxShift.Models;

namespace BoxShift.Services;

public class CustomLevelService
{
    private readonly string _filePath;

    public CustomLevelService()
    {
        _filePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "customlevels.json");
    }

    public async Task<List<Level>> LoadCustomLevelsAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Level>();
        }

        string json =
            await File.ReadAllTextAsync(_filePath);

        List<Level>? levels =
            JsonSerializer.Deserialize<List<Level>>(json);

        return levels ?? new List<Level>();
    }

    public async Task SaveCustomLevelAsync(Level level)
    {
        List<Level> levels =
            await LoadCustomLevelsAsync();

        levels.Add(level);

        await SaveAllCustomLevelsAsync(levels);
    }

    public async Task UpdateCustomLevelAsync(
        int index,
        Level updatedLevel)
    {
        List<Level> levels =
            await LoadCustomLevelsAsync();

        if (index < 0 ||
            index >= levels.Count)
        {
            return;
        }

        levels[index] = updatedLevel;

        await SaveAllCustomLevelsAsync(levels);
    }

    public async Task DeleteCustomLevelAsync(
        int index)
    {
        List<Level> levels =
            await LoadCustomLevelsAsync();

        if (index < 0 ||
            index >= levels.Count)
        {
            return;
        }

        levels.RemoveAt(index);

        await SaveAllCustomLevelsAsync(levels);
    }

    private async Task SaveAllCustomLevelsAsync(
        List<Level> levels)
    {
        string json =
            JsonSerializer.Serialize(
                levels,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        await File.WriteAllTextAsync(
            _filePath,
            json);
    }
}