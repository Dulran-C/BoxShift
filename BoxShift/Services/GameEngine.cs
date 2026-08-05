using BoxShift.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        int cols = level.Rows.Max(r => r.Length);

        char[,] grid = new char[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                char tile = ' ';

                if (col < level.Rows[row].Length)
                    tile = level.Rows[row][col];

                grid[row, col] = tile;

                if (tile == '@')
                {
                    PlayerPosition = new Position(row, col);
                }
            }
        }

        Board = new GameBoard(grid);
    }
}
