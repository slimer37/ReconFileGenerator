using System;
using System.IO;

namespace ReconFileGenerator;

public static class ReconFileFormatter
{
    public struct Info
    {
        public string scramble;
        public string inspectionRotation;
        
        public string crossColor;
        public string crossMoves;
        public int xCrossCount;
        
        public string ollPreAuf;
        public string ollShape;
        public bool ollSkip;
        public string ollMoves;
        
        public string pllPreAuf;
        public string pllPerm;
        public bool pllSkip;
        public string pllMoves;

        public string auf;
        
        public float totalTime;
        public string cubeName;
        public DateTime dateTime;

        public string GetOLLString() => ollSkip ? "Skip" : ollShape + '\n' + (string.IsNullOrEmpty(ollPreAuf) ? "" : ollPreAuf + ' ') + ollMoves;
        public string GetPLLString() => pllSkip ? "Skip" : pllPerm + " Perm\n" + (string.IsNullOrEmpty(pllPreAuf) ? "" : pllPreAuf + ' ') + pllMoves;

        public string GetXCrossString()
        {
            if (xCrossCount <= 0) return "";
            
            var x = "";
            for (var i = 0; i < xCrossCount; i++) x += "X";
            return x + '-';
        }
        
        public int GetSTLTotalMoves()
        {
            var allMoves = string.Join(' ', crossMoves, ollMoves, pllMoves);
            
            if (string.IsNullOrEmpty(allMoves)) return 0;
            
            var total = 0;
            
            foreach (var move in allMoves.Split(' '))
                if (Cube.IsValidMove(move) && !"xyz".Contains(char.ToLower(move[0])))
                    total++;
            
            return total;
        }

        public string GetTPS()
        {
            var result = GetSTLTotalMoves() / totalTime;
            if (float.IsNaN(result) || float.IsInfinity(result))
                return "0";
            return result.ToString("#0.##");
        }
    }

    public static string GetText(Info i)
    {
        return $@"{i.scramble}

// Inspection
{i.inspectionRotation}
// {i.crossColor} {i.GetXCrossString()}Cross
{i.crossMoves}
// F2L [Edge Color/Edge Color]
(include the two colors of the edge solved e.g. F2L RedWhite)
(repeat F2L step for any F2L pairs remaining)
// OLL {i.GetOLLString()}
// PLL {i.GetPLLString()}
{(i.pllSkip ? "// AUF\n" + i.auf : "")}
{i.totalTime} 3x3 Single
{i.GetSTLTotalMoves()} Moves STM {i.GetTPS()}tps
{i.cubeName}
{(i.dateTime == default ? "No date set" : i.dateTime.ToString("M/d/yy"))}";
    }

    public static void CreateTextFile(Info i, string path)
    {
        using var stream = File.CreateText(path);
        stream.Write(GetText(i));
    }
}
