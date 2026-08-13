using System.Diagnostics;
using BoxShift.Models;
using BoxShift.Services;

namespace BoxShift.Pages;

[QueryProperty(nameof(LevelIndex), "levelIndex")]
[QueryProperty(nameof(LevelSource), "levelSource")]
public partial class GamePage : ContentPage
{
    private readonly GameEngine _gameEngine;
    private readonly LevelService _levelService;
    private readonly ProgressService _progressService;
    private readonly CustomLevelService _customLevelService;

    public string LevelIndex { get; set; } = "0";

    public string LevelSource { get; set; } =
        "builtin";

    public GamePage()
    {
        InitializeComponent();

        _gameEngine = new GameEngine();
        _levelService = new LevelService();
        _progressService = new ProgressService();
        _customLevelService =
            new CustomLevelService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_gameEngine.Board == null)
        {
            await LoadGameAsync();
        }
    }

    private async Task LoadGameAsync()
    {
        try
        {
            int selectedIndex;

            if (!int.TryParse(
                    LevelIndex,
                    out selectedIndex))
            {
                selectedIndex = 0;
            }

            Level? selectedLevel = null;

            if (LevelSource == "custom")
            {
                List<Level> customLevels =
                    await _customLevelService
                        .LoadCustomLevelsAsync();

                if (selectedIndex >= 0 &&
                    selectedIndex < customLevels.Count)
                {
                    selectedLevel =
                        customLevels[selectedIndex];
                }
            }
            else
            {
                LevelCollection? builtInLevels =
                    await _levelService
                        .LoadLevelsAsync();

                if (builtInLevels != null &&
                    selectedIndex >= 0 &&
                    selectedIndex <
                    builtInLevels.Levels.Count)
                {
                    selectedLevel =
                        builtInLevels
                            .Levels[selectedIndex];
                }
            }

            if (selectedLevel == null)
            {
                LevelTitle.Text =
                    "Level could not be loaded";

                return;
            }

            _gameEngine.LoadLevel(
                selectedLevel);

            DisplayBoard();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(
                $"Could not load level: {exception.Message}");

            LevelTitle.Text =
                "Could not load level";
        }
    }

    private void DisplayBoard()
    {
        if (_gameEngine.Board == null)
        {
            return;
        }

        LevelTitle.Text =
            _gameEngine.CurrentLevel?.Name
            ?? "Level";

        MoveCounter.Text =
            $"Moves: {_gameEngine.MoveCount}";

        BoardGrid.Children.Clear();
        BoardGrid.RowDefinitions.Clear();
        BoardGrid.ColumnDefinitions.Clear();

        int rows =
            _gameEngine.Board.Rows;

        int columns =
            _gameEngine.Board.Columns;

        for (int row = 0;
             row < rows;
             row++)
        {
            BoardGrid.RowDefinitions.Add(
                new RowDefinition(
                    GridLength.Auto));
        }

        for (int column = 0;
             column < columns;
             column++)
        {
            BoardGrid.ColumnDefinitions.Add(
                new ColumnDefinition(
                    GridLength.Auto));
        }

        for (int row = 0;
             row < rows;
             row++)
        {
            for (int column = 0;
                 column < columns;
                 column++)
            {
                char tile =
                    _gameEngine.Board
                        .GetCell(
                            row,
                            column);

                Label tileLabel =
                    new Label
                    {
                        Text =
                            tile.ToString(),

                        FontSize = 22,
                        WidthRequest = 32,
                        HeightRequest = 32,

                        HorizontalTextAlignment =
                            TextAlignment.Center,

                        VerticalTextAlignment =
                            TextAlignment.Center
                    };

                Grid.SetRow(
                    tileLabel,
                    row);

                Grid.SetColumn(
                    tileLabel,
                    column);

                BoardGrid.Children.Add(
                    tileLabel);
            }
        }
    }

    private async Task MovePlayerAsync(
        Direction direction)
    {
        bool moved =
            _gameEngine.Move(direction);

        if (!moved)
        {
            return;
        }

        DisplayBoard();

        if (_gameEngine.IsLevelComplete())
        {
            LevelTitle.Text =
                "Level Complete!";

            if (LevelSource == "builtin" &&
                int.TryParse(
                    LevelIndex,
                    out int selectedIndex))
            {
                await _progressService
                    .RecordCompletionAsync(
                        selectedIndex,
                        _gameEngine.MoveCount);
            }
        }
    }

    private void UndoClicked(
        object sender,
        EventArgs e)
    {
        bool undone =
            _gameEngine.Undo();

        if (undone)
        {
            DisplayBoard();
        }
    }

    private void ResetClicked(
        object sender,
        EventArgs e)
    {
        _gameEngine.ResetLevel();

        DisplayBoard();
    }

    private async void UpClicked(
        object sender,
        EventArgs e)
    {
        await MovePlayerAsync(
            Direction.Up);
    }

    private async void DownClicked(
        object sender,
        EventArgs e)
    {
        await MovePlayerAsync(
            Direction.Down);
    }

    private async void LeftClicked(
        object sender,
        EventArgs e)
    {
        await MovePlayerAsync(
            Direction.Left);
    }

    private async void RightClicked(
        object sender,
        EventArgs e)
    {
        await MovePlayerAsync(
            Direction.Right);
    }
}