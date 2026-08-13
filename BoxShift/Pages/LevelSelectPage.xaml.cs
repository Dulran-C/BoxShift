using System.Diagnostics;
using BoxShift.Models;
using BoxShift.Services;

namespace BoxShift.Pages;

public partial class LevelSelectPage : ContentPage
{
    private readonly LevelService _levelService;
    private readonly ProgressService _progressService;

    public LevelSelectPage()
    {
        InitializeComponent();

        _levelService = new LevelService();
        _progressService = new ProgressService();
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
            LevelCollection? levelCollection =
                await _levelService.LoadLevelsAsync();

            if (levelCollection == null)
            {
                return;
            }

            List<LevelProgress> progress =
                await _progressService.LoadProgressAsync();

            List<LevelSelectItem> levelItems =
                new List<LevelSelectItem>();

            for (int index = 0;
                 index < levelCollection.Levels.Count;
                 index++)
            {
                Level level =
                    levelCollection.Levels[index];

                LevelProgress? savedProgress =
                    progress.FirstOrDefault(
                        item => item.LevelIndex == index);

                LevelSelectItem item =
                    new LevelSelectItem
                    {
                        LevelIndex = index,
                        Name = level.Name,
                        IsCompleted =
                            savedProgress?.IsCompleted ?? false,
                        BestMoves =
                            savedProgress?.BestMoves ?? 0
                    };

                levelItems.Add(item);
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

        if (button.CommandParameter is not int levelIndex)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(GamePage)}?levelIndex={levelIndex}");
    }
}