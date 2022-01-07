using System.IO;

namespace ReconFileGenerator;

public static class ReconFileFormatter
{
    public struct Info
    {
        public string scramble;
    }

    public static bool IsValidMove(string move) =>
        move.Length is 1 or 2
        && "rludfb".Contains(char.ToLower(move[0]))
        && (move.Length == 1 || "2'".Contains(move[1]));
    
    public static void CreateTextFile(Info i, string path)
    {
        using (var stream = File.CreateText(path))
        {
            stream.Write(
$@"{i.scramble}


// Inspection
(any rotation from scramble state to the desired starting orientation of
the cube before starting the timer)
// [Color] Cross
(Specify cross color and x-crosses e.g. Blue XX-Cross)

// F2L [Edge Color/Edge Color]
(include the two colors of the edge solved e.g. F2L RedWhite)
(repeat F2L step for any F2L pairs remaining)

// OLL [#] [Shape/Type] / OLL Skip
(e.g. OLL 57 Corners Oriented)
(include pre-AUF)

// PLL [Letter Name] Perm / PLL Skip
(e.g. PLL Gc Perm)
(include pre-AUF and AUF)

// AUF (Only use after PLL Skip and if there is AUF)

[Time (ss:XX)] 3x3 Single
(e.g. 9.29 3x3 Single)
[Move Count] Moves STM
(All moves are counted by Slice Turn Metric)
[X]tps
(e.g. 6.89tps)
(Move Count/Time)
[Name of Cube (Brand|Model)](e.g. Qiyi Valk M)
[Date of Solve]"
            );
        }
    }
}
