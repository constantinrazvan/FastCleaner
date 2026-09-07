using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace FastCleanerWin
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<CleanerFile> Files { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void DropZone_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;

            e.Handled = true;
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (paths.Length == 0)
                return;

            string executablePath = GetExecutablePath(paths[0]);

            if (string.IsNullOrWhiteSpace(executablePath) || !File.Exists(executablePath))
            {
                MessageBox.Show(
                    "The selected file is not a valid application executable.",
                    "FastCleaner",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            string appFolder = Path.GetDirectoryName(executablePath)!;

            LoadApplicationFiles(appFolder, executablePath);

            DropPanel.Visibility = Visibility.Collapsed;
            ResultsPanel.Visibility = Visibility.Visible;
            ClearButton.Visibility = Visibility.Visible;
        }

        private string GetExecutablePath(string droppedPath)
        {
            if (Path.GetExtension(droppedPath).Equals(".exe",
                StringComparison.OrdinalIgnoreCase))
            {
                return droppedPath;
            }

            if (Path.GetExtension(droppedPath).Equals(".lnk",
                StringComparison.OrdinalIgnoreCase))
            {
                Type shellType = Type.GetTypeFromProgID("WScript.Shell")!;
                dynamic shell = Activator.CreateInstance(shellType)!;
                dynamic shortcut = shell.CreateShortcut(droppedPath);

                return shortcut.TargetPath as string ?? "";
            }

            return "";
        }

        private void LoadApplicationFiles(string appFolder, string executablePath)
        {
            Files.Clear();

            foreach (string filePath in Directory.GetFiles(
                appFolder,
                "*",
                SearchOption.AllDirectories))
            {
                FileInfo file = new FileInfo(filePath);

                Files.Add(new CleanerFile
                {
                    IsSelected = true,
                    Name = file.Name,
                    FullPath = file.FullName,
                    LastModified = file.LastWriteTime.ToString("dd.MM.yyyy HH:mm"),
                    Size = FormatSize(file.Length)
                });
            }

            StatusText.Text =
                $"{Files.Count} files for application: {Path.GetFileName(executablePath)}";

            DeleteButton.Visibility = Files.Any(file => file.IsSelected)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private string LogFilePath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "FastCleaner",
                    "deletion-history.log");

        private void SaveDeletionToHistory(CleanerFile file)
        {
            string? folder = Path.GetDirectoryName(LogFilePath);

            if (folder != null)
                Directory.CreateDirectory(folder);

            string logLine =
                $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} | {file.FullPath} | {file.Size}";

            File.AppendAllText(LogFilePath, logLine + Environment.NewLine);
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(LogFilePath))
            {
                MessageBox.Show("No deletions in history yet.", "History");
                return;
            }

            string[] entries = File.ReadAllLines(LogFilePath)
                .TakeLast(30)
                .Reverse()
                .ToArray();

            MessageBox.Show(
                string.Join(Environment.NewLine, entries),
                "Recent Deletions");
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFiles = Files.Where(file => file.IsSelected).ToList();

            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("No files selected.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete {selectedFiles.Count} files?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            foreach (var file in selectedFiles)
            {
                if (File.Exists(file.FullPath))
                    File.Delete(file.FullPath);

                Files.Remove(file);
            }

            StatusText.Text = "The selected files have been deleted.";
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024d:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / 1024d / 1024d:F1} MB";

            return $"{bytes / 1024d / 1024d / 1024d:F2} GB";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Files.Clear();

            ResultsPanel.Visibility = Visibility.Collapsed;
            DropPanel.Visibility = Visibility.Visible;

            ClearButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;

            StatusText.Text = "Drag the application executable or shortcut here.";
        }
    }
}