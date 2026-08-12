using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using BoxShift.Models;

namespace BoxShift.Services;

public class LevelService
{
    private const string LevelFile = "levels.json";

    public async Task<LevelCollection?> LoadLevelsAsync()
    {
        using Stream stream =
            await FileSystem.OpenAppPackageFileAsync(LevelFile);

        return await JsonSerializer.DeserializeAsync<LevelCollection>(
            stream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}