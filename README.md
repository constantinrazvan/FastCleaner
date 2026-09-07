
![Logo](https://i.imgur.com/zxG7WOc.png)


# FastCleanerWin
FastCleaner is a lightweight, open-source Windows alternative to AppCleaner for macOS, designed to help users identify and remove leftover files from unwanted applications.

Users can drag and drop an application executable (`.exe`) or shortcut (`.lnk`) into the app. FastCleaner resolves the real executable location, scans the application’s installation folder, and displays the related files in a clear table.

For each detected file, the user can review:

- File name
- Full location
- Last modified date
- File size
- Selection status

Users can choose which files to remove, clear the current scan to analyze another app, and view a history of previous removals. The interface is simple, modern, and focused on giving users control before anything is deleted.


## Installation

1. Clone the repository:

```bash
git clone https://github.com/your-username/FastCleaner.git
cd FastCleaner
```

2. Open `FastCleaner.sln` in Visual Studio.

3. Build and run the project:

```bash
dotnet build
dotnet run --project FastCleanerWin
```

Requirements: Windows, .NET SDK, and Visual Studio with the **.NET desktop development** workload.

## Features
- Drag and drop support for `.exe` files and application shortcuts
- Automatically resolves shortcuts to the original executable
- Scans the selected application's installation folder
- Displays file name, full path, last modified date, and size
- Select or deselect individual files before removal
- Clean button to reset the current scan and add another application
- Removal history for previously deleted files
- Modern, lightweight Windows interface

## 🛠 Skills

- C#
- .NET
- WPF
- XAML
- Windows File System APIs
- Windows Registry
- JSON
- Git

## FAQ

#### What files does FastCleaner scan?
FastCleaner scans the installation folder of the dropped application executable. If you drop a shortcut, it first identifies the real `.exe` file and then scans that app’s folder.

#### Can FastCleaner delete files automatically?
No. FastCleaner shows all detected files first. You choose which files to select, and deletion requires confirmation.

#### Does FastCleaner scan my entire computer?
No. It only scans the folder associated with the selected application, helping avoid unrelated files such as items on the Desktop.

#### What does the Clean button do?
Clean clears the current results and returns to the drag-and-drop screen. It does not delete any files.

#### Can I see what was removed?
Yes. The History section keeps a record of previous file removals.

#### Why can’t some files be removed?
Some files may be in use or protected by Windows. Try closing the related application or running FastCleaner as Administrator.

## Contributing

Contributions are welcome!

To get started, please read [CONTRIBUTING.md](CONTRIBUTING.md). It explains how to report issues, suggest features, and submit pull requests.

Please be respectful and keep contributions focused on making FastCleaner safer and more useful.