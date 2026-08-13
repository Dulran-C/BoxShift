using System.Diagnostics;
using BoxShift.Helpers;
using BoxShift.Models;
using BoxShift.Services;
using Microsoft.Maui.Controls.Shapes;

namespace BoxShift.Pages;

[QueryProperty(nameof(LevelIndex), "levelIndex")]
[QueryProperty(nameof(LevelSource), "levelSource")]
public partial class GamePage : ContentPage
{
    private readonly GameEngine _gameEngine;
    private readonly LevelService _levelService;
    private readonly ProgressService _progressService;
    private readonly CustomLevelService _customLevelService;
    private readonly SettingsService _settingsService;

    public string LevelIndex { get; set; } = "0";

    public string LevelSource { get; set; } =
        "builtin";

    public GamePage()
    {
        InitializeComponent();

        _gameEngine =
            new GameEngine();

        _levelService =
            new LevelService();

        _progressService =
            new ProgressService();

        _customLevelService =
            new CustomLevelService();

        _settingsService =
            new SettingsService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_gameEngine.Board == null)
        {
            await LoadGameAsync();
        }
        else
        {
            DisplayBoard();
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

                View tileView =
                    CreateTileView(tile);

                Grid.SetRow(
                    tileView,
                    row);

                Grid.SetColumn(
                    tileView,
                    column);

                BoardGrid.Children.Add(
                    tileView);
            }
        }
    }

    private View CreateTileView(
        char tile)
    {
        var colors =
            GetThemeColors();

        Color backgroundColor =
            colors.Floor;

        Color strokeColor =
            colors.Edge;

        Color textColor =
            Colors.White;

        string text = "";

        double strokeThickness = 1;

        if (tile == GameSymbols.Wall)
        {
            backgroundColor =
                colors.Wall;

            strokeColor =
                colors.Wall;
        }
        else if (tile == GameSymbols.Target)
        {
            backgroundColor =
                colors.Target;

            text = "X";

            textColor =
                colors.DarkText;
        }
        else if (tile == GameSymbols.Box)
        {
            backgroundColor =
                colors.Box;

            text = "B";
        }
        else if (tile == GameSymbols.Player)
        {
            backgroundColor =
                colors.Player;

            text = "P";
        }
        else if (tile == GameSymbols.BoxOnTarget)
        {
            backgroundColor =
                colors.Box;

            strokeColor =
                colors.Target;

            strokeThickness = 4;

            text = "B";
        }
        else if (tile == GameSymbols.PlayerOnTarget)
        {
            backgroundColor =
                colors.Player;

            strokeColor =
                colors.Target;

            strokeThickness = 4;

            text = "P";
        }

        Label tileLabel =
            new Label
            {
                Text = text,

                FontSize = 16,

                FontAttributes =
                    FontAttributes.Bold,

                TextColor =
                    textColor,

                HorizontalTextAlignment =
                    TextAlignment.Center,

                VerticalTextAlignment =
                    TextAlignment.Center
            };

        Border tileBorder =
            new Border
            {
                WidthRequest = 32,
                HeightRequest = 32,

                Padding = 0,

                Background =
                    new SolidColorBrush(
                        backgroundColor),

                Stroke =
                    new SolidColorBrush(
                        strokeColor),

                StrokeThickness =
                    strokeThickness,

                StrokeShape =
                    new RoundRectangle
                    {
                        CornerRadius =
                            new CornerRadius(5)
                    },

                Content =
                    tileLabel
            };

        return tileBorder;
    }

    private (
        Color Floor,
        Color Wall,
        Color Box,
        Color Player,
        Color Target,
        Color Edge,
        Color DarkText)
        GetThemeColors()
    {
        string theme =
            _settingsService.GridTheme;

        if (theme == "Ocean")
        {
            return (
                Color.FromArgb("#DDF4F7"),
                Color.FromArgb("#155E75"),
                Color.FromArgb("#F59E72"),
                Color.FromArgb("#0284C7"),
                Color.FromArgb("#7DD3FC"),
                Color.FromArgb("#164E63"),
                Color.FromArgb("#12313A"));
        }

        if (theme == "Forest")
        {
            return (
                Color.FromArgb("#E8F0E4"),
                Color.FromArgb("#355E3B"),
                Color.FromArgb("#A56A43"),
                Color.FromArgb("#2F855A"),
                Color.FromArgb("#C6D57E"),
                Color.FromArgb("#294D31"),
                Color.FromArgb("#243B2A"));
        }

        return (
            Color.FromArgb("#EEEAE2"),
            Color.FromArgb("#374151"),
            Color.FromArgb("#C98545"),
            Color.FromArgb("#5746E8"),
            Color.FromArgb("#F2CE67"),
            Color.FromArgb("#4B5563"),
            Color.FromArgb("#352E1F"));
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

            await ShowCompletionAsync();
        }
    }

    private Task ShowCompletionAsync()
    {
        LevelTitle.Text =
            "Level Complete!";

        if (!_settingsService.AnimationsEnabled)
        {
            return Task.CompletedTask;
        }

        LevelTitle.Scale = 0.85;
        LevelTitle.Opacity = 0.4;

        Animation completionAnimation =
            new Animation();

        completionAnimation.Add(
            0,
            0.6,
            new Animation(
                value => LevelTitle.Scale = value,
                0.85,
                1.15));

        completionAnimation.Add(
            0,
            0.6,
            new Animation(
                value => LevelTitle.Opacity = value,
                0.4,
                1.0));

        completionAnimation.Add(
            0.6,
            1,
            new Animation(
                value => LevelTitle.Scale = value,
                1.15,
                1.0));

        completionAnimation.Commit(
            LevelTitle,
            "LevelCompleteAnimation",
            16,
            320,
            Easing.CubicOut);

        return Task.CompletedTask;
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