using BoxShift.Helpers;
using BoxShift.Models;
using BoxShift.Services;

namespace BoxShift.Pages;

public partial class EditorPage : ContentPage
{
    private char[,] _editorGrid = new char[5, 5];

    private char _selectedTool =
        GameSymbols.Wall;

    private GameEngine? _testEngine;

    private bool _levelValidated;

    private readonly CustomLevelService _customLevelService;

    public EditorPage()
    {
        InitializeComponent();

        _customLevelService =
            new CustomLevelService();

        List<int> sizes =
            Enumerable.Range(5, 8).ToList();

        RowsPicker.ItemsSource = sizes;
        ColumnsPicker.ItemsSource = sizes;

        RowsPicker.SelectedItem = 5;
        ColumnsPicker.SelectedItem = 5;

        CreateBlankGrid(5, 5);
    }

    private void CreateGridClicked(
        object sender,
        EventArgs e)
    {
        int rows =
            (int)(RowsPicker.SelectedItem ?? 5);

        int columns =
            (int)(ColumnsPicker.SelectedItem ?? 5);

        CreateBlankGrid(rows, columns);

        EditorStatus.Text =
            "New grid created.";
    }

    private void CreateBlankGrid(
        int rows,
        int columns)
    {
        _editorGrid =
            new char[rows, columns];

        for (int row = 0;
             row < rows;
             row++)
        {
            for (int column = 0;
                 column < columns;
                 column++)
            {
                _editorGrid[row, column] =
                    GameSymbols.Floor;
            }
        }

        _levelValidated = false;
        SaveButton.IsEnabled = false;

        DisplayEditorGrid();
    }

    private void ToolClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        string? value =
            button.CommandParameter?.ToString();

        if (string.IsNullOrEmpty(value))
        {
            _selectedTool =
                GameSymbols.Floor;

            SelectedToolLabel.Text =
                "Selected: Floor";

            return;
        }

        _selectedTool = value[0];

        SelectedToolLabel.Text =
            $"Selected: {button.Text}";
    }

    private void DisplayEditorGrid()
    {
        EditorGrid.Children.Clear();
        EditorGrid.RowDefinitions.Clear();
        EditorGrid.ColumnDefinitions.Clear();

        int rows =
            _editorGrid.GetLength(0);

        int columns =
            _editorGrid.GetLength(1);

        for (int row = 0;
             row < rows;
             row++)
        {
            EditorGrid.RowDefinitions.Add(
                new RowDefinition(
                    GridLength.Auto));
        }

        for (int column = 0;
             column < columns;
             column++)
        {
            EditorGrid.ColumnDefinitions.Add(
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
                Button cellButton =
                    new Button
                    {
                        Text =
                            _editorGrid[row, column]
                            .ToString(),

                        WidthRequest = 45,
                        HeightRequest = 45,

                        Padding = 0,

                        CommandParameter =
                            $"{row},{column}"
                    };

                cellButton.Clicked +=
                    EditorCellClicked;

                Grid.SetRow(
                    cellButton,
                    row);

                Grid.SetColumn(
                    cellButton,
                    column);

                EditorGrid.Children.Add(
                    cellButton);
            }
        }
    }

    private void EditorCellClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        string? position =
            button.CommandParameter?.ToString();

        if (position == null)
        {
            return;
        }

        string[] parts =
            position.Split(',');

        int row =
            int.Parse(parts[0]);

        int column =
            int.Parse(parts[1]);

        if (_selectedTool ==
            GameSymbols.Player)
        {
            RemoveExistingPlayer();
        }

        _editorGrid[row, column] =
            _selectedTool;

        _levelValidated = false;
        SaveButton.IsEnabled = false;

        EditorStatus.Text =
            "Level changed. Test again before saving.";

        DisplayEditorGrid();
    }

    private void RemoveExistingPlayer()
    {
        int rows =
            _editorGrid.GetLength(0);

        int columns =
            _editorGrid.GetLength(1);

        for (int row = 0;
             row < rows;
             row++)
        {
            for (int column = 0;
                 column < columns;
                 column++)
            {
                if (_editorGrid[row, column] ==
                    GameSymbols.Player)
                {
                    _editorGrid[row, column] =
                        GameSymbols.Floor;
                }
            }
        }
    }

    private void TestLevelClicked(
        object sender,
        EventArgs e)
    {
        Level? level =
            CreateLevelFromEditor();

        if (level == null)
        {
            return;
        }

        _testEngine =
            new GameEngine();

        _testEngine.LoadLevel(level);

        EditorTools.IsVisible = false;
        TestButton.IsVisible = false;

        TestControls.IsVisible = true;
        CancelTestButton.IsVisible = true;

        EditorStatus.Text =
            "Test mode: solve your level.";

        DisplayTestGrid();
    }

    private Level? CreateLevelFromEditor()
    {
        int playerCount = 0;
        int boxCount = 0;
        int targetCount = 0;

        List<string> rows =
            new List<string>();

        for (int row = 0;
             row < _editorGrid.GetLength(0);
             row++)
        {
            char[] rowCharacters =
                new char[
                    _editorGrid.GetLength(1)];

            for (int column = 0;
                 column < _editorGrid.GetLength(1);
                 column++)
            {
                char tile =
                    _editorGrid[row, column];

                rowCharacters[column] =
                    tile;

                if (tile ==
                    GameSymbols.Player)
                {
                    playerCount++;
                }

                if (tile ==
                    GameSymbols.Box)
                {
                    boxCount++;
                }

                if (tile ==
                    GameSymbols.Target)
                {
                    targetCount++;
                }
            }

            rows.Add(
                new string(rowCharacters));
        }

        if (playerCount != 1)
        {
            EditorStatus.Text =
                "Level must contain exactly one player.";

            return null;
        }

        if (boxCount == 0)
        {
            EditorStatus.Text =
                "Level must contain at least one box.";

            return null;
        }

        if (boxCount != targetCount)
        {
            EditorStatus.Text =
                "Boxes and targets must match.";

            return null;
        }

        string levelName =
            LevelNameEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(levelName))
        {
            levelName = "Custom Level";
        }

        return new Level
        {
            Name = levelName,
            Rows = rows
        };
    }

    private void DisplayTestGrid()
    {
        if (_testEngine?.Board == null)
        {
            return;
        }

        EditorGrid.Children.Clear();
        EditorGrid.RowDefinitions.Clear();
        EditorGrid.ColumnDefinitions.Clear();

        for (int row = 0;
             row < _testEngine.Board.Rows;
             row++)
        {
            EditorGrid.RowDefinitions.Add(
                new RowDefinition(
                    GridLength.Auto));
        }

        for (int column = 0;
             column < _testEngine.Board.Columns;
             column++)
        {
            EditorGrid.ColumnDefinitions.Add(
                new ColumnDefinition(
                    GridLength.Auto));
        }

        for (int row = 0;
             row < _testEngine.Board.Rows;
             row++)
        {
            for (int column = 0;
                 column < _testEngine.Board.Columns;
                 column++)
            {
                Label cellLabel =
                    new Label
                    {
                        Text =
                            _testEngine.Board
                            .GetCell(row, column)
                            .ToString(),

                        WidthRequest = 45,
                        HeightRequest = 45,
                        FontSize = 25,

                        HorizontalTextAlignment =
                            TextAlignment.Center,

                        VerticalTextAlignment =
                            TextAlignment.Center
                    };

                Grid.SetRow(
                    cellLabel,
                    row);

                Grid.SetColumn(
                    cellLabel,
                    column);

                EditorGrid.Children.Add(
                    cellLabel);
            }
        }
    }

    private void MoveTestPlayer(
        Direction direction)
    {
        if (_testEngine == null)
        {
            return;
        }

        bool moved =
            _testEngine.Move(direction);

        if (!moved)
        {
            return;
        }

        DisplayTestGrid();

        if (_testEngine.IsLevelComplete())
        {
            _levelValidated = true;

            SaveButton.IsEnabled = true;

            EditorStatus.Text =
                "Test passed! You can save this level.";

            TestControls.IsVisible = false;
            CancelTestButton.IsVisible = false;
            EditorTools.IsVisible = true;
            TestButton.IsVisible = true;

            DisplayEditorGrid();
        }
    }

    private void CancelTestClicked(
        object sender,
        EventArgs e)
    {
        _testEngine = null;

        TestControls.IsVisible = false;
        CancelTestButton.IsVisible = false;

        EditorTools.IsVisible = true;
        TestButton.IsVisible = true;

        EditorStatus.Text =
            "Test stopped. Level was not validated.";

        DisplayEditorGrid();
    }

    private async void SaveLevelClicked(
        object sender,
        EventArgs e)
    {
        if (!_levelValidated)
        {
            EditorStatus.Text =
                "You must complete the test first.";

            return;
        }

        Level? level =
            CreateLevelFromEditor();

        if (level == null)
        {
            return;
        }

        await _customLevelService
            .SaveCustomLevelAsync(level);

        EditorStatus.Text =
            "Custom level saved!";

        SaveButton.IsEnabled = false;

        _levelValidated = false;
    }

    private void TestUpClicked(
        object sender,
        EventArgs e)
    {
        MoveTestPlayer(Direction.Up);
    }

    private void TestDownClicked(
        object sender,
        EventArgs e)
    {
        MoveTestPlayer(Direction.Down);
    }

    private void TestLeftClicked(
        object sender,
        EventArgs e)
    {
        MoveTestPlayer(Direction.Left);
    }

    private void TestRightClicked(
        object sender,
        EventArgs e)
    {
        MoveTestPlayer(Direction.Right);
    }
}