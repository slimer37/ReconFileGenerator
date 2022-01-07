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

namespace SolveFileGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SaveFileDialog saveFileDialog = new() { Filter = "Text File (*.txt)|*.txt" };
        
        public MainWindow() => InitializeComponent();

        void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (saveFileDialog.ShowDialog() ?? false)
            {
                OverwriteWarning.Visibility = File.Exists(saveFileDialog.FileName) ? Visibility.Visible : Visibility.Collapsed;
                SaveFilePath.Text = "Will Save To: " + saveFileDialog.FileName;
            }
        }
        
        void ScrambleText_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            FilePreview.Text = ((TextBox)sender).Text;
        }
    }
}
