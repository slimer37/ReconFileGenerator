using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ReconFileGenerator;

public static class F2LHandler
{
    public static void GenerateFromComboBoxes(Cube.Color crossColor, params ComboBox[] dropdowns)
    {
        if (dropdowns.Length % 2 != 0)
            throw new ArgumentException("Number of dropdowns is not even.", nameof(dropdowns));

        var usedOnce = new List<string>();
        var faceSet = GetF2LRing(crossColor).ToList();
        for (var i = 0; i < dropdowns.Length; i++)
        {
            var dropdown = dropdowns[i];
            dropdown.Replace(faceSet);
            
            if (i % 2 != 0 && dropdowns[i - 1].SelectedItem != null)
                dropdown.Items.Remove((Cube.Color)dropdowns[i - 1].SelectedItem);
        }
    }

    static void Replace(this ComboBox dropdown, IEnumerable set)
    {
        if (dropdown.SelectedIndex != -1)
        {
            for (var i = 0; i < dropdown.Items.Count; i++)
            {
                if (i == dropdown.SelectedIndex) continue;
                dropdown.Items.RemoveAt(i);
            }
        }
        else if (dropdown.Items.Count > 0)
            dropdown.Items.Clear();

        var selected = dropdown.SelectedItem;
        foreach (var item in set)
        {
            if (selected != null && item == selected) continue;
            dropdown.Items.Add(item);
        }
    }

    static IEnumerable<Cube.Color> GetF2LRing(Cube.Color crossColor)
    {
        var allColors = Enum.GetValues<Cube.Color>().ToList();
        allColors.Remove(crossColor);
        allColors.Remove(Cube.Opposites[crossColor]);
        return allColors.ToArray();
    }
}
