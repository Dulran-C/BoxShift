using System.Diagnostics;
using BoxShift.Models;
using BoxShift.Services;

namespace BoxShift.Pages;

public partial class GamePage : ContentPage
{
    private readonly GameEngine _gameEngine;
    private readonly LevelService _levelService;

    public GamePage()
    {
        InitializeComponent();

        _gameEngine = new GameEngine();
        _levelService = new LevelService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadGameAsync();
    }

    private async Task LoadGameAsync()
    {
        try
        {
            LevelCollection? levels =
                await _levelService.LoadLevelsAsync();

            if (levels == null ||
                levels.Levels.Count == 0)
            {
                LevelTitle.Text =
                    "No levels found";

                return;
            }

            _gameEngine.LoadFirstLevel(levels);

            DisplayBoard();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(
                $"Could not load levels: {exception.Message}");

            LevelTitle.Text =
                "Could not load levels";
        }
    }

    private void DisplayBoard()
    {
        if (_gameEngine.Board == null)
        {
            return;
        }

        LevelTitle.Text =
            _gameEngine.CurrentLevel?.Name ?? "Level";

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
                    _gameEngine.Board.GetCell(
                        row,
                        column);

                Label tileLabel = new Label
                {
                    Text = tile.ToString(),
                    FontSize = 30,
                    WidthRequest = 40,
                    HeightRequest = 40,

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

    private void MovePlayer(
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
                "Level Complete! 🎉";
        }
    }

    private void UpClicked(
        object sender,
        EventArgs e)
    {
        MovePlayer(Direction.Up);
    }

    private void DownClicked(
        object sender,
        EventArgs e)
    {
        MovePlayer(Direction.Down);
    }

    private void LeftClicked(
        object sender,
        EventArgs e)
    {
        MovePlayer(Direction.Left);
    }

    private void RightClicked(
        object sender,
        EventArgs e)
    {
        MovePlayer(Direction.Right);
    }
}