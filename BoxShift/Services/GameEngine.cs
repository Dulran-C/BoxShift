using BoxShift.Helpers;
using BoxShift.Models;

namespace BoxShift.Services;

public class GameEngine
{
    public Level? CurrentLevel { get; private set; }

    public GameBoard? Board { get; private set; }

    public Position PlayerPosition { get; private set; } = new();

    public int MoveCount { get; private set; }

    public void LoadLevel(Level level)
    {
        CurrentLevel = level;
        MoveCount = 0;

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
                    PlayerPosition = new Position(row, column);
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

        int newRow = PlayerPosition.Row + rowChange;
        int newColumn = PlayerPosition.Column + columnChange;

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

        // We will add box pushing in the next section.
        if (destination == GameSymbols.Box ||
            destination == GameSymbols.BoxOnTarget)
        {
            return false;
        }

        char currentTile =
            Board.GetCell(
                PlayerPosition.Row,
                PlayerPosition.Column);

        if (currentTile == GameSymbols.PlayerOnTarget)
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

        if (destination == GameSymbols.Target)
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

        PlayerPosition = new Position(
            newRow,
            newColumn);

        MoveCount++;

        return true;
    }

    private bool IsInsideBoard(int row, int column)
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