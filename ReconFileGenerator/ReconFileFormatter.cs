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

        string[] f2L;

        public void GenerateF2LText(int index, string colors, string moves)
        {
            f2L ??= new string[4];
            f2L[index] = $"//F2L {colors} \n{moves}";
        }
        
        public string GetF2LString() => f2L == null ? "" : string.Join('\n', f2L);

        public string GetOllString() =>
            ollSkip ? "Skip" : ollShape + '\n' + (string.IsNullOrEmpty(ollPreAuf) ? "" : ollPreAuf + ' ') + ollMoves;
        public string GetPllString() =>
            pllSkip ? "Skip" : pllPerm + " Perm\n" + (string.IsNullOrEmpty(pllPreAuf) ? "" : pllPreAuf + ' ') + pllMoves + ' ' + auf;

        public string GetXCrossString()
        {
            if (xCrossCount <= 0) return "";
            
            var x = "";
            for (var i = 0; i < xCrossCount; i++) x += "X";
            return x + '-';
        }
        
        public int GetStmTotalMoves()
        {
            var allMoves = string.Join(' ', crossMoves, ollMoves, pllMoves, ollPreAuf, pllPreAuf, auf,
                string.Join(' ', f2L ?? Array.Empty<string>()));
            
            if (string.IsNullOrEmpty(allMoves)) return 0;
            
            var total = 0;
            
            foreach (var move in allMoves.Split(' '))
            {
                if (string.IsNullOrWhiteSpace(move)) continue;
                var modified = move.Replace("\n", "");
                if (Cube.IsValidMove(modified) && !"xyz".Contains(char.ToLower(modified[0])))
                    total++;
            }
            
            return total;
        }

        public string GetTps()
        {
            var result = GetStmTotalMoves() / totalTime;
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
{i.GetF2LString()}
// OLL {i.GetOllString()}
// PLL {i.GetPllString()}
{(i.pllSkip ? "// AUF\n" + i.auf : "")}
{i.totalTime} 3x3 Single
{i.GetStmTotalMoves()} Moves STM {i.GetTps()}tps
{i.cubeName}
{(i.dateTime == default ? "No date set" : i.dateTime.ToString("M/d/yy"))}";
    }

    public static void CreateTextFile(Info i, string path)
    {
        using var stream = File.CreateText(path);
        stream.Write(GetText(i));
    }
}
