using System.Diagnostics;
using System.Text.Json;
using BoxShift.Models;

namespace BoxShift.Services;

public class LevelService
{
    private const string LevelFile = "levels.json";

    private const string LevelDownloadUrl =
        "https://raw.githubusercontent.com/Dulran-C/BoxShift/refs/heads/master/BoxShift/Resources/Raw/levels.json";

    private readonly string _localFilePath;

    public LevelService()
    {
        _localFilePath = Path.Combine(
            FileSystem.AppDataDirectory,
            LevelFile);
    }

    public async Task<LevelCollection?> LoadLevelsAsync()
    {
        if (File.Exists(_localFilePath))
        {
            return await LoadLocalLevelsAsync();
        }

        try
        {
            await DownloadLevelsAsync();

            return await LoadLocalLevelsAsync();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(
                $"Level download failed: {exception.Message}");

            return await LoadPackagedLevelsAsync();
        }
    }

    private async Task DownloadLevelsAsync()
    {
        using HttpClient client =
            new HttpClient();

        string json =
            await client.GetStringAsync(
                LevelDownloadUrl);

        LevelCollection? levels =
            DeserializeLevels(json);

        if (levels == null ||
            levels.Levels.Count == 0)
        {
            throw new InvalidOperationException(
                "Downloaded level file contained no levels.");
        }

        await File.WriteAllTextAsync(
            _localFilePath,
            json);

        Debug.WriteLine(
            "Levels downloadedd and saved locally.");
    }

    private async Task<LevelCollection?>
        LoadLocalLevelsAsync()
    {
        string json =
            await File.ReadAllTextAsync(
                _localFilePath);

        return DeserializeLevels(json);
    }

    private async Task<LevelCollection?>
        LoadPackagedLevelsAsync()
    {
        using Stream stream =
            await FileSystem
                .OpenAppPackageFileAsync(
                    LevelFile);

        using StreamReader reader =
            new StreamReader(stream);

        string json =
            await reader.ReadToEndAsync();

        return DeserializeLevels(json);
    }

    private LevelCollection?
        DeserializeLevels(string json)
    {
        return JsonSerializer
            .Deserialize<LevelCollection>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive =
                        true
                });
    }
}