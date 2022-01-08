using System;
using System.Collections.Generic;
using System.Linq;

namespace ReconFileGenerator;

public static class Cube
{
    public enum Color
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        White
    }
    
    public static readonly Dictionary<Color, Color> Opposites = new()
    {
        { Color.Red, Color.Orange },
        { Color.Orange, Color.Red },
        { Color.Yellow, Color.White },
        { Color.White, Color.Yellow },
        { Color.Green, Color.Blue },
        { Color.Blue, Color.Green }
    };

    public static char Singular(Color color) => color.ToString()[0];
    
    public static bool IsValidMove(string move) =>
        move.Length is 1 or 2
        && "rludfbxyz".Contains(char.ToLower(move[0]))
        && (move.Length == 1 || "2'".Contains(move[1]));
    
    public static Color[] GetF2LSet(Color crossColor)
    {
        var allColors = Enum.GetValues(typeof(Color)).Cast<Color>().ToList();
        allColors.Remove(crossColor);
        allColors.Remove(Opposites[crossColor]);
        return allColors.ToArray();
    }
}
