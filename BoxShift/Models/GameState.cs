using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BoxShift.Models;

public class GameState
{
    public char[,] Grid { get; set; }

    public Position PlayerPosition { get; set; }

    public int MoveCount { get; set; }

    public GameState(
        char[,] grid,
        Position playerPosition,
        int moveCount)
    {
        Grid = grid;
        PlayerPosition = playerPosition;
        MoveCount = moveCount;
    }
}
