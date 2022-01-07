using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ReconFileGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SaveFileDialog saveFileDialog = new() { Filter = "Text File (*.txt)|*.txt" };
        ReconFileFormatter.Info info = new();
        
        public MainWindow() => InitializeComponent();

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
                    if (move.Length != 0 && !ReconFileFormatter.IsValidMove(move))
                    {
                        anyInvalid = true;
                        ScrambleTextInvalidWarning.Text = $"\"{move}\" doesn't look right.";
                    }

                ScrambleTextInvalidWarning.Visibility = anyInvalid ? Visibility.Visible : Visibility.Collapsed;
            }
            
            FilePreview.Text = scrambleText;
        }
    }
}
