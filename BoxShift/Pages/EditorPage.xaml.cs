using BoxShift.Helpers;
using BoxShift.Models;
using BoxShift.Services;
using Microsoft.Maui.Controls.Shapes;

namespace BoxShift.Pages;

[QueryProperty(nameof(CustomIndex), "customIndex")]
public partial class EditorPage : ContentPage
{
    private char[,] _editorGrid =
        new char[5, 5];

    private char _selectedTool =
        GameSymbols.Wall;

    private GameEngine? _testEngine;

    private bool _levelValidated;

    private bool _existingLevelLoaded;

    private int _editingIndex = -1;

    private readonly CustomLevelService
        _customLevelService;

    private readonly SettingsService
        _settingsService;

    public string CustomIndex { get; set; } =
        "-1";

    public EditorPage()
    {
        InitializeComponent();

        _customLevelService =
            new CustomLevelService();

        _settingsService =
            new SettingsService();

        List<int> sizes =
            Enumerable.Range(5, 8).ToList();

        RowsPicker.ItemsSource = sizes;
        ColumnsPicker.ItemsSource = sizes;

        RowsPicker.SelectedItem = 5;
        ColumnsPicker.SelectedItem = 5;

        CreateBlankGrid(5, 5);

        UpdateToolSelection();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_existingLevelLoaded)
        {
            await LoadExistingLevelAsync();
        }
        else if (_testEngine == null)
        {
            DisplayEditorGrid();
        }
    }

    private async Task LoadExistingLevelAsync()
    {
        _existingLevelLoaded = true;

        if (!int.TryParse(
                CustomIndex,
                out int index))
        {
            return;
        }

        if (index < 0)
        {
            return;
        }

        List<Level> levels =
            await _customLevelService
                .LoadCustomLevelsAsync();

        if (index >= levels.Count)
        {
            return;
        }

        _editingIndex = index;

        Level level =
            levels[index];

        LevelNameEntry.Text =
            level.Name;

        int rows =
            level.Rows.Count;

        int columns =
            level.Rows.Max(
                row => row.Length);

        RowsPicker.SelectedItem = rows;
        ColumnsPicker.SelectedItem = columns;

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
                char tile =
                    GameSymbols.Floor;

                if (column <
                    level.Rows[row].Length)
                {
                    tile =
                        level.Rows[row][column];
                }

                _editorGrid[row, column] =
                    tile;
            }
        }

        _levelValidated = false;

        SaveButton.IsEnabled = false;
        SaveButton.Text = "Update Level";

        EditorStatus.Text =
            "Editing custom level. Test before saving changes.";

        DisplayEditorGrid();
    }

    private void CreateGridClicked(
        object? sender,
        EventArgs e)
    {
        int rows =
            (int)(RowsPicker.SelectedItem ?? 5);

        int columns =
            (int)(ColumnsPicker.SelectedItem ?? 5);

        CreateBlankGrid(
            rows,
            columns);

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

        if (SaveButton != null)
        {
            SaveButton.IsEnabled = false;
        }

        DisplayEditorGrid();
    }

    private void ToolClicked(
        object? sender,
        EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        string tool =
            button.CommandParameter?.ToString()
            ?? "Wall";

        if (tool == "Floor")
        {
            _selectedTool =
                GameSymbols.Floor;
        }
        else if (tool == "Box")
        {
            _selectedTool =
                GameSymbols.Box;
        }
        else if (tool == "Target")
        {
            _selectedTool =
                GameSymbols.Target;
        }
        else if (tool == "Player")
        {
            _selectedTool =
                GameSymbols.Player;
        }
        else
        {
            _selectedTool =
                GameSymbols.Wall;
        }

        SelectedToolLabel.Text =
            $"Selected: {tool}";

        UpdateToolSelection();
    }

    private void UpdateToolSelection()
    {
        Button[] buttons =
        {
            WallToolButton,
            FloorToolButton,
            BoxToolButton,
            TargetToolButton,
            PlayerToolButton
        };

        foreach (Button button in buttons)
        {
            button.BorderWidth = 0;
            button.Opacity = 0.75;
        }

        Button selectedButton =
            _selectedTool switch
            {
                GameSymbols.Floor =>
                    FloorToolButton,

                GameSymbols.Box =>
                    BoxToolButton,

                GameSymbols.Target =>
                    TargetToolButton,

                GameSymbols.Player =>
                    PlayerToolButton,

                _ =>
                    WallToolButton
            };

        selectedButton.BorderColor =
            Color.FromArgb("#5746E8");

        selectedButton.BorderWidth = 3;
        selectedButton.Opacity = 1;
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

        double cellSize =
            GetCellSize(columns);

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
                char tile =
                    _editorGrid[
                        row,
                        column];

                var colors =
                    GetThemeColors();

                Button cellButton =
                    new Button
                    {
                        Text =
                            GetTileText(tile),

                        FontSize = 14,

                        FontAttributes =
                            FontAttributes.Bold,

                        WidthRequest =
                            cellSize,

                        HeightRequest =
                            cellSize,

                        Padding = 0,

                        CornerRadius = 5,

                        BackgroundColor =
                            GetTileBackground(
                                tile,
                                colors),

                        TextColor =
                            GetTileTextColor(
                                tile,
                                colors),

                        BorderColor =
                            GetTileBorderColor(
                                tile,
                                colors),

                        BorderWidth = 1,

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
        object? sender,
        EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        string? position =
            button.CommandParameter
                ?.ToString();

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
                if (_editorGrid[
                        row,
                        column] ==
                    GameSymbols.Player)
                {
                    _editorGrid[
                        row,
                        column] =
                        GameSymbols.Floor;
                }
            }
        }
    }

    private void TestLevelClicked(
        object? sender,
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

        CancelTestButton.IsVisible =
            true;

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
             row <
             _editorGrid.GetLength(0);
             row++)
        {
            char[] rowCharacters =
                new char[
                    _editorGrid
                        .GetLength(1)];

            for (int column = 0;
                 column <
                 _editorGrid.GetLength(1);
                 column++)
            {
                char tile =
                    _editorGrid[
                        row,
                        column];

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
                new string(
                    rowCharacters));
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
            LevelNameEntry.Text?.Trim()
            ?? "";

        if (string.IsNullOrWhiteSpace(
                levelName))
        {
            levelName =
                "Custom Level";
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

        int columns =
            _testEngine.Board.Columns;

        double cellSize =
            GetCellSize(columns);

        for (int row = 0;
             row <
             _testEngine.Board.Rows;
             row++)
        {
            EditorGrid.RowDefinitions.Add(
                new RowDefinition(
                    GridLength.Auto));
        }

        for (int column = 0;
             column <
             _testEngine.Board.Columns;
             column++)
        {
            EditorGrid.ColumnDefinitions.Add(
                new ColumnDefinition(
                    GridLength.Auto));
        }

        for (int row = 0;
             row <
             _testEngine.Board.Rows;
             row++)
        {
            for (int column = 0;
                 column <
                 _testEngine.Board.Columns;
                 column++)
            {
                char tile =
                    _testEngine.Board
                        .GetCell(
                            row,
                            column);

                View tileView =
                    CreateDisplayTile(
                        tile,
                        cellSize);

                Grid.SetRow(
                    tileView,
                    row);

                Grid.SetColumn(
                    tileView,
                    column);

                EditorGrid.Children.Add(
                    tileView);
            }
        }
    }

    private View CreateDisplayTile(
        char tile,
        double cellSize)
    {
        var colors =
            GetThemeColors();

        Color backgroundColor =
            GetTileBackground(
                tile,
                colors);

        Color strokeColor =
            GetTileBorderColor(
                tile,
                colors);

        double strokeThickness = 1;

        if (tile ==
                GameSymbols.BoxOnTarget ||
            tile ==
                GameSymbols.PlayerOnTarget)
        {
            strokeColor =
                colors.Target;

            strokeThickness = 4;
        }

        Label label =
            new Label
            {
                Text =
                    GetTileText(tile),

                FontSize = 14,

                FontAttributes =
                    FontAttributes.Bold,

                TextColor =
                    GetTileTextColor(
                        tile,
                        colors),

                HorizontalTextAlignment =
                    TextAlignment.Center,

                VerticalTextAlignment =
                    TextAlignment.Center
            };

        Border border =
            new Border
            {
                WidthRequest =
                    cellSize,

                HeightRequest =
                    cellSize,

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
                    label
            };

        return border;
    }

    private string GetTileText(
        char tile)
    {
        if (tile ==
                GameSymbols.Player ||
            tile ==
                GameSymbols.PlayerOnTarget)
        {
            return "P";
        }

        if (tile ==
                GameSymbols.Box ||
            tile ==
                GameSymbols.BoxOnTarget)
        {
            return "B";
        }

        if (tile ==
            GameSymbols.Target)
        {
            return "X";
        }

        return "";
    }

    private Color GetTileBackground(
        char tile,
        (
            Color Floor,
            Color Wall,
            Color Box,
            Color Player,
            Color Target,
            Color Edge,
            Color DarkText)
        colors)
    {
        if (tile ==
            GameSymbols.Wall)
        {
            return colors.Wall;
        }

        if (tile ==
                GameSymbols.Box ||
            tile ==
                GameSymbols.BoxOnTarget)
        {
            return colors.Box;
        }

        if (tile ==
                GameSymbols.Player ||
            tile ==
                GameSymbols.PlayerOnTarget)
        {
            return colors.Player;
        }

        if (tile ==
            GameSymbols.Target)
        {
            return colors.Target;
        }

        return colors.Floor;
    }

    private Color GetTileBorderColor(
        char tile,
        (
            Color Floor,
            Color Wall,
            Color Box,
            Color Player,
            Color Target,
            Color Edge,
            Color DarkText)
        colors)
    {
        if (tile ==
                GameSymbols.BoxOnTarget ||
            tile ==
                GameSymbols.PlayerOnTarget)
        {
            return colors.Target;
        }

        if (tile ==
            GameSymbols.Wall)
        {
            return colors.Wall;
        }

        return colors.Edge;
    }

    private Color GetTileTextColor(
        char tile,
        (
            Color Floor,
            Color Wall,
            Color Box,
            Color Player,
            Color Target,
            Color Edge,
            Color DarkText)
        colors)
    {
        if (tile ==
            GameSymbols.Target)
        {
            return colors.DarkText;
        }

        return Colors.White;
    }

    private double GetCellSize(
        int columns)
    {
        if (columns >= 11)
        {
            return 24;
        }

        if (columns >= 9)
        {
            return 28;
        }

        if (columns >= 7)
        {
            return 32;
        }

        return 38;
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

        if (_testEngine
            .IsLevelComplete())
        {
            _levelValidated = true;

            SaveButton.IsEnabled =
                true;

            EditorStatus.Text =
                "Test passed! You can save this level.";

            TestControls.IsVisible =
                false;

            CancelTestButton.IsVisible =
                false;

            EditorTools.IsVisible =
                true;

            TestButton.IsVisible =
                true;

            DisplayEditorGrid();
        }
    }

    private void CancelTestClicked(
        object? sender,
        EventArgs e)
    {
        _testEngine = null;

        TestControls.IsVisible =
            false;

        CancelTestButton.IsVisible =
            false;

        EditorTools.IsVisible =
            true;

        TestButton.IsVisible =
            true;

        EditorStatus.Text =
            "Test stopped. Level was not validated.";

        DisplayEditorGrid();
    }

    private async void SaveLevelClicked(
        object? sender,
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

        if (_editingIndex >= 0)
        {
            await _customLevelService
                .UpdateCustomLevelAsync(
                    _editingIndex,
                    level);

            EditorStatus.Text =
                "Custom level updated!";
        }
        else
        {
            await _customLevelService
                .SaveCustomLevelAsync(
                    level);

            EditorStatus.Text =
                "Custom level saved!";
        }

        SaveButton.IsEnabled =
            false;

        _levelValidated =
            false;
    }

    private void TestUpClicked(
        object? sender,
        EventArgs e)
    {
        MoveTestPlayer(
            Direction.Up);
    }

    private void TestDownClicked(
        object? sender,
        EventArgs e)
    {
        MoveTestPlayer(
            Direction.Down);
    }

    private void TestLeftClicked(
        object? sender,
        EventArgs e)
    {
        MoveTestPlayer(
            Direction.Left);
    }

    private void TestRightClicked(
        object? sender,
        EventArgs e)
    {
        MoveTestPlayer(
            Direction.Right);
    }
}