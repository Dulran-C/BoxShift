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