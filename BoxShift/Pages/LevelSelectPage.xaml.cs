using System.Diagnostics;
using BoxShift.Models;
using BoxShift.Services;

namespace BoxShift.Pages;

public partial class LevelSelectPage : ContentPage
{
    private readonly LevelService _levelService;
    private readonly ProgressService _progressService;
    private readonly CustomLevelService _customLevelService;

    public LevelSelectPage()
    {
        InitializeComponent();

        _levelService = new LevelService();
        _progressService = new ProgressService();
        _customLevelService =
            new CustomLevelService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadLevelsAsync();
    }

    private async Task LoadLevelsAsync()
    {
        try
        {
            LevelCollection? builtInLevels =
                await _levelService.LoadLevelsAsync();

            List<LevelProgress> progress =
                await _progressService.LoadProgressAsync();

            List<Level> customLevels =
                await _customLevelService
                    .LoadCustomLevelsAsync();

            List<LevelSelectItem> levelItems =
                new List<LevelSelectItem>();

            if (builtInLevels != null)
            {
                for (int index = 0;
                     index < builtInLevels.Levels.Count;
                     index++)
                {
                    Level level =
                        builtInLevels.Levels[index];

                    LevelProgress? savedProgress =
                        progress.FirstOrDefault(
                            item =>
                                item.LevelIndex == index);

                    levelItems.Add(
                        new LevelSelectItem
                        {
                            LevelIndex = index,
                            Name = level.Name,
                            IsCustom = false,

                            IsCompleted =
                                savedProgress?.IsCompleted
                                ?? false,

                            BestMoves =
                                savedProgress?.BestMoves
                                ?? 0
                        });
                }
            }

            for (int index = 0;
                 index < customLevels.Count;
                 index++)
            {
                levelItems.Add(
                    new LevelSelectItem
                    {
                        LevelIndex = index,
                        Name = customLevels[index].Name,
                        IsCustom = true
                    });
            }

            LevelsCollection.ItemsSource =
                levelItems;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(
                $"Could not load level list: {exception.Message}");
        }
    }

    private async void PlayLevelClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.CommandParameter
            is not LevelSelectItem selectedItem)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(GamePage)}" +
            $"?levelIndex={selectedItem.LevelIndex}" +
            $"&levelSource={selectedItem.LevelSource}");
    }

    private async void EditorClicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(EditorPage));
    }

    private async void EditLevelClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.CommandParameter
            is not int levelIndex)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(EditorPage)}" +
            $"?customIndex={levelIndex}");
    }

    private async void DeleteLevelClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.CommandParameter
            is not int levelIndex)
        {
            return;
        }

        await _customLevelService
            .DeleteCustomLevelAsync(levelIndex);

        await LoadLevelsAsync();
    }
}