using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxShift.Models
{
    public class GameBoard
    {
        public char[,] Grid { get; }

        public int Rows => Grid.GetLength(0);

        public int Columns => Grid.GetLength(1);

        public GameBoard(char[,] grid)
        {
            Grid = grid;
        }

        public char GetCell(int row, int column)
        {
            return Grid[row, column];
        }

        public void SetCell(int row, int column, char value)
        {
            Grid[row, column] = value;
        }
    }
}
