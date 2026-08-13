using BoxShift.Helpers;
using BoxShift.Models;

namespace BoxShift.Services;

public class GameEngine
{
    public Level? CurrentLevel { get; private set; }

    public GameBoard? Board { get; private set; }

    public Position PlayerPosition { get; private set; } = new();

    public int MoveCount { get; private set; }

    private readonly Stack<GameState> _undoHistory = new();

    public void LoadLevel(Level level)
    {
        CurrentLevel = level;
        MoveCount = 0;

        _undoHistory.Clear();

        int rows = level.Rows.Count;
        int columns = level.Rows.Max(row => row.Length);

        char[,] grid = new char[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                char tile = GameSymbols.Floor;

                if (column < level.Rows[row].Length)
                {
                    tile = level.Rows[row][column];
                }

                grid[row, column] = tile;

                if (tile == GameSymbols.Player ||
                    tile == GameSymbols.PlayerOnTarget)
                {
                    PlayerPosition =
                        new Position(row, column);
                }
            }
        }

        Board = new GameBoard(grid);
    }

    public void LoadFirstLevel(LevelCollection levels)
    {
        if (levels.Levels.Count > 0)
        {
            LoadLevel(levels.Levels[0]);
        }
    }

    public bool Move(Direction direction)
    {
        if (Board == null)
        {
            return false;
        }

        int rowChange = 0;
        int columnChange = 0;

        switch (direction)
        {
            case Direction.Up:
                rowChange = -1;
                break;

            case Direction.Down:
                rowChange = 1;
                break;

            case Direction.Left:
                columnChange = -1;
                break;

            case Direction.Right:
                columnChange = 1;
                break;
        }

        int newRow =
            PlayerPosition.Row + rowChange;

        int newColumn =
            PlayerPosition.Column + columnChange;

        if (!IsInsideBoard(newRow, newColumn))
        {
            return false;
        }

        char destination =
            Board.GetCell(newRow, newColumn);

        if (destination == GameSymbols.Wall)
        {
            return false;
        }

        GameState previousState = CreateGameState();

        if (destination == GameSymbols.Box ||
            destination == GameSymbols.BoxOnTarget)
        {
            bool boxMoved = TryPushBox(
                newRow,
                newColumn,
                rowChange,
                columnChange);

            if (!boxMoved)
            {
                return false;
            }
        }

        _undoHistory.Push(previousState);

        MovePlayerTo(
            newRow,
            newColumn,
            destination);

        MoveCount++;

        return true;
    }

    private bool TryPushBox(
        int boxRow,
        int boxColumn,
        int rowChange,
        int columnChange)
    {
        if (Board == null)
        {
            return false;
        }

        int newBoxRow =
            boxRow + rowChange;

        int newBoxColumn =
            boxColumn + columnChange;

        if (!IsInsideBoard(
                newBoxRow,
                newBoxColumn))
        {
            return false;
        }

        char boxDestination =
            Board.GetCell(
                newBoxRow,
                newBoxColumn);

        if (boxDestination != GameSymbols.Floor &&
            boxDestination != GameSymbols.Target)
        {
            return false;
        }

        if (boxDestination == GameSymbols.Target)
        {
            Board.SetCell(
                newBoxRow,
                newBoxColumn,
                GameSymbols.BoxOnTarget);
        }
        else
        {
            Board.SetCell(
                newBoxRow,
                newBoxColumn,
                GameSymbols.Box);
        }

        return true;
    }

    private void MovePlayerTo(
        int newRow,
        int newColumn,
        char destination)
    {
        if (Board == null)
        {
            return;
        }

        char currentTile =
            Board.GetCell(
                PlayerPosition.Row,
                PlayerPosition.Column);

        if (currentTile ==
            GameSymbols.PlayerOnTarget)
        {
            Board.SetCell(
                PlayerPosition.Row,
                PlayerPosition.Column,
                GameSymbols.Target);
        }
        else
        {
            Board.SetCell(
                PlayerPosition.Row,
                PlayerPosition.Column,
                GameSymbols.Floor);
        }

        if (destination ==
            GameSymbols.BoxOnTarget)
        {
            Board.SetCell(
                newRow,
                newColumn,
                GameSymbols.PlayerOnTarget);
        }
        else
        {
            Board.SetCell(
                newRow,
                newColumn,
                GameSymbols.Player);
        }

        PlayerPosition =
            new Position(
                newRow,
                newColumn);
    }

    public bool Undo()
    {
        if (_undoHistory.Count == 0)
        {
            return false;
        }

        GameState previousState =
            _undoHistory.Pop();

        Board =
            new GameBoard(previousState.Grid);

        PlayerPosition =
            previousState.PlayerPosition.Copy();

        MoveCount =
            previousState.MoveCount;

        return true;
    }

    public void ResetLevel()
    {
        if (CurrentLevel != null)
        {
            LoadLevel(CurrentLevel);
        }
    }

    public bool IsLevelComplete()
    {
        if (Board == null)
        {
            return false;
        }

        for (int row = 0;
             row < Board.Rows;
             row++)
        {
            for (int column = 0;
                 column < Board.Columns;
                 column++)
            {
                if (Board.GetCell(row, column) ==
                    GameSymbols.Box)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private GameState CreateGameState()
    {
        if (Board == null)
        {
            throw new InvalidOperationException(
                "Cannot save game state without a board.");
        }

        char[,] gridCopy =
            new char[Board.Rows, Board.Columns];

        for (int row = 0;
             row < Board.Rows;
             row++)
        {
            for (int column = 0;
                 column < Board.Columns;
                 column++)
            {
                gridCopy[row, column] =
                    Board.GetCell(row, column);
            }
        }

        return new GameState(
            gridCopy,
            PlayerPosition.Copy(),
            MoveCount);
    }

    private bool IsInsideBoard(
        int row,
        int column)
    {
        if (Board == null)
        {
            return false;
        }

        return row >= 0 &&
               row < Board.Rows &&
               column >= 0 &&
               column < Board.Columns;
    }
}