using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Color = ReconFileGenerator.Cube.Color;

namespace ReconFileGenerator;

public static class F2LHandler
{
    static Color colorCache = (Color)(-1);
    static bool inProgress;
    
    public static void GenerateFromComboBoxes(Color crossColor, params ComboBox[] dropdowns)
    {
        if (inProgress) return;
        inProgress = true;
        
        if (dropdowns.Length % 2 != 0)
            throw new ArgumentException("Number of dropdowns is not even.", nameof(dropdowns));

        var faceSet = GetF2LRing(crossColor).ToList();
        for (var i = 0; i < dropdowns.Length; i++)
        {
            var dropdown = dropdowns[i];
            
            if (colorCache != crossColor)
                dropdown.Replace(faceSet);
            
            // On the second dropdown of each pair, remove the color selected in its partner
            if (i % 2 != 0 && dropdowns[i - 1].SelectedItem != null)
            {
                dropdown.Replace(faceSet);
                dropdown.Items.Remove((Color)dropdowns[i - 1].SelectedItem);
            }
        }

        colorCache = crossColor;

        inProgress = false;
    }

    static void Replace(this ComboBox dropdown, IEnumerable set)
    {
        var selected = dropdown.SelectedItem;
        var selIndex = dropdown.SelectedIndex;

        if (selIndex != -1 && !set.Cast<object?>().Contains(selected))
            selIndex = -1;
        
        dropdown.Items.Clear();
        
        foreach (var item in set)
        {
            if (selected != null && item.ToString() == selected.ToString()) continue;
            dropdown.Items.Add(item);
        }
        
        if (selIndex != -1)
        {
            dropdown.Items.Insert(selIndex, selected);
            dropdown.SelectedIndex = selIndex;
        }
    }

    static IEnumerable<Color> GetF2LRing(Color crossColor)
    {
        var allColors = Enum.GetValues<Color>().ToList();
        allColors.Remove(crossColor);
        allColors.Remove(Cube.Opposites[crossColor]);
        return allColors.ToArray();
    }
}
