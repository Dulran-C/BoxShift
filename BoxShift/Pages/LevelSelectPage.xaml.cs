using System.Diagnostics;
using BoxShift.Models;
using BoxShift.Services;

namespace BoxShift.Pages;

public partial class LevelSelectPage : ContentPage
{
    private readonly LevelService _levelService;

    private List<Level> _levels;

    public LevelSelectPage()
    {
        InitializeComponent();

        _levelService = new LevelService();
        _levels = new List<Level>();
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

            _levels = levelCollection.Levels;

            LevelsCollection.ItemsSource = _levels;
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

        if (button.CommandParameter is not Level selectedLevel)
        {
            return;
        }

        int levelIndex =
            _levels.IndexOf(selectedLevel);

        if (levelIndex < 0)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(GamePage)}?levelIndex={levelIndex}");
    }
}