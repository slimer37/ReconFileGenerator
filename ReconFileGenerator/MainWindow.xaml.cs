﻿using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ReconFileGenerator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    SaveFileDialog saveFileDialog = new() { Filter = "Text File (*.txt)|*.txt" };
    ReconFileFormatter.Info info;
    
    const int AttachParentProcess = -1;

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern bool AttachConsole(int dwProcessId);
    static void AttachToParentConsole() => AttachConsole(AttachParentProcess);
    
    public MainWindow()
    {
        AttachToParentConsole();
        InitializeComponent();
        UpdatePreview();
    }

    void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (saveFileDialog.ShowDialog() ?? false)
        {
            ReconFileFormatter.CreateTextFile(info, saveFileDialog.FileName);
            SavedText.Visibility = Visibility.Visible;
            SavedText.Text = $"Created {saveFileDialog.SafeFileName}";
        }
    }

    void ScrambleText_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var scrambleText = ScrambleText.Text;
        
        if (!string.IsNullOrEmpty(scrambleText))
        {
            var anyInvalid = false;
            foreach (var move in scrambleText.Split(' '))
                if (move.Length != 0 && !Cube.IsValidMove(move))
                {
                    anyInvalid = true;
                    ScrambleTextInvalidWarning.Text = $"\"{move}\" doesn't look right.";
                }

            ScrambleTextInvalidWarning.Visibility = anyInvalid ? Visibility.Visible : Visibility.Collapsed;
        }
        
        General_OnTextChanged(sender, e);
    }
    
    void General_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Time.Text) && !float.TryParse(Time.Text.Replace(':', '.'), out info.totalTime))
            TimeFormatWarning.Visibility = Visibility.Visible;
        else
            TimeFormatWarning.Visibility = Visibility.Collapsed;
        
        info.scramble = ScrambleText.Text;
        info.inspectionRotation = InspectionText.Text;
        info.crossMoves = CrossMoves.Text;
        info.cubeName = CubeName.Text;
        
        info.ollPreAuf = OLLPreAUF.Text;
        info.ollMoves = OLLMoves.Text;
        
        info.pllPreAuf = PLLPreAUF.Text;
        info.pllMoves = PLLMoves.Text;

        info.auf = AUF.Text;

        UpdatePreview();
    }

    void RefreshF2LDropdowns() => F2LHandler.GenerateFromComboBoxes(Enum.Parse<Cube.Color>(info.crossColor),
        F2L11, F2L12, F2L21, F2L22, F2L31, F2L32, F2L41, F2L42);

    void CrossColor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CrossColor.SelectedItem == null) return;

        info.crossColor = CrossColor.SelectedItem.ToString() ?? "";
        
        RefreshF2LDropdowns();
        UpdatePreview();
    }

    void F2L_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var dropdown = (ComboBox)sender;
        var otherPart = int.Parse(dropdown.Name[^1].ToString()) % 2 + 1;
        var otherDropdownQuery = dropdown.Name[..^1] + otherPart;
        var otherDropdown = (ComboBox)(FindName(otherDropdownQuery)
                                       ?? throw new InvalidOperationException("Couldn't find " + otherDropdownQuery));

        var moveBox = dropdown.Name[..^1] + 'M';
        var moves = (TextBox)(FindName(moveBox) ?? throw new InvalidOperationException("Couldn't find " + moveBox));

        var col1 = dropdown.SelectedItem?.ToString() ?? "";
        var col2 = otherDropdown.SelectedItem?.ToString() ?? "";
        
        info.GenerateF2LText(int.Parse(dropdown.Name[^2].ToString()) - 1, col1 + col2, moves.Text);
        RefreshF2LDropdowns();
        UpdatePreview();
    }

    void UpdatePreview()
    {
        if (Stats == null || FilePreview == null) return;
        Stats.Text = $"Total Moves (STM): {info.GetStmTotalMoves()}\nTPS: {info.GetTps()}";
        FilePreview.Text = ReconFileFormatter.GetText(info);
    }
    
    void Date_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (Date.SelectedDate == null) return;
        info.dateTime = (DateTime)Date.SelectedDate;
        UpdatePreview();
    }
    
    void XCross_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        info.xCrossCount = XCross.SelectedIndex;
        UpdatePreview();
    }
    
    void OLLShape_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (OLLShape.SelectedItem == null) return;
        info.ollShape = OLLShape.SelectedItem.ToString() ?? "";
        UpdatePreview();
    }
    
    void PLLPerm_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PLLPerm.SelectedItem == null) return;
        info.pllPerm = PLLPerm.SelectedItem.ToString() ?? "";
        UpdatePreview();
    }
    
    void OLLSkip_OnClick(object sender, RoutedEventArgs e)
    {
        info.ollSkip = OLLSkip.IsChecked ?? false;
        UpdatePreview();
    }
    
    void PLLSkip_OnClick(object sender, RoutedEventArgs e)
    {
        info.pllSkip = PLLSkip.IsChecked ?? false;
        UpdatePreview();
    }
    
    void CrossColor_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var color in Enum.GetNames<Cube.Color>())
            CrossColor.Items.Add(color);
    }
    
    void OLLShape_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var shape in Cube.OllShapes) OLLShape.Items.Add(shape);
    }
    
    void PLLPerm_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var perm in Cube.PllPerms) PLLPerm.Items.Add(perm);
    }
}