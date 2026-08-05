using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxShift.Models;

namespace BoxShift.Services;

public class GameEngine
{
    public Level? CurrentLevel { get; private set; }

    public Position PlayerPosition { get; private set; } = new();

    public int MoveCount { get; private set; }

    public void LoadLevel(Level level)
    {
        CurrentLevel = level;
        MoveCount = 0;
    }
}
