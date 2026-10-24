# Glass Notes - Transparent Notes App for Windows

![Glass Notes Homepage](Assets/screenshots/homepage.png)

Glass Notes (GlassNotes) is a lightweight, **open-source desktop notes app for Windows 10/11**. It keeps notes in a customizable transparent window and combines free-form notes with an interactive to-do list. Glass Notes is built with WPF and .NET 10.

### Download

<a href="https://apps.microsoft.com/store/detail/9N5SVJGDBHH2?cid=DevShareMCLPCS">
  <img src="https://get.microsoft.com/images/en-us%20dark.svg" width="200" alt="Get it from Microsoft"/>
</a>

## Features

- **Always On Top** - Keep your notes visible while working in other applications
- **Customizable Appearance** - Choose between light and dark themes
- **Adjustable Transparency** - Set the perfect opacity level (10%-100%)
- **Hide Notes** - Quickly toggle the visibility of all your notes to declutter your workspace
- **Multiple Notes** - Create and manage multiple notes with quick switching
  ![Multiple Tabs Example](Assets/screenshots/open_multiple_tabs.png)
- **Interactive To-Do Lists** - Press Enter to add a bulleted item; complete, pin, or delete items; completed items move to the bottom with strikethrough
- **Keyboard Editing** - Press Backspace on an empty to-do item to remove it
- **Custom Typography** - Choose the font family and size for notes and to-do items
- **Launch at Sign-In** - Optionally start Glass Notes when you sign in to Windows
- **Auto-Save** - Your notes are automatically saved every 3 seconds
- **Clean, Modern Interface** - Built with Windows Fluent Design principles
- **Lightweight & Fast** - Minimal resource usage, instant startup
- **Resizable Windows** - Adjust size and position to fit your workflow

## Requirements

- Windows 10 (version 1809 or later) or Windows 11
- To build from source: .NET 10.0 SDK
- The self-contained Windows x64 publish does not require a separate .NET runtime installation

## Building from Source

### Prerequisites

- .NET 10.0 SDK

### Build Instructions

1. Clone or download this repository
2. Open a terminal in the project directory
3. Run the following commands:

```powershell
cd GlassNotes
dotnet restore
dotnet build
```

### Run the Application

```powershell
dotnet run
```

Or build a release version:

```powershell
dotnet build --configuration Release
```

The executable will be in: `bin\Release\net10.0-windows\GlassNotes.exe`.

To create a self-contained single-file Windows x64 executable:

```powershell
dotnet publish .\GlassNotes.csproj -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:PublishReadyToRun=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:DebugType=None -p:DebugSymbols=false -o .\publish-single-exe
```

The output folder contains a single `GlassNotes.exe` that can run on compatible Windows x64 computers without installing the .NET runtime.

## Usage

### Basic Controls

- **Drag the title bar** to move the window
- **Hide/Show Notes** by clicking the eye icon or pressing `Ctrl + Shift + Z`
- **Click the gear icon** to open settings
- **Click New** to create a new note
- **Add to-dos** with the plus button in the bottom toolbar, or press Enter at the end of a to-do
- **Complete, pin, or delete a to-do** using the actions shown when hovering over its row
- **Press Backspace** in an empty to-do row to remove it
- **Type directly** in the note or to-do area - changes auto-save

### Settings

![Settings Menu](Assets/screenshots/settings.png)
- **Opacity**: Adjust window transparency (10%-100%)
- **Font Size**: Change text size (10-24pt)
- **Font Family**: Choose the typeface for note and to-do text
- **Theme**: Switch between Light and Dark modes
- **Always on Top**: Toggle whether window stays above other windows
- **Launch at Sign-In**: Start the app automatically for the current Windows user

## Data Storage

Notes and settings are stored in:
```
%APPDATA%\GlassNotes\
├── Notes\           # Individual note files (.json)
└── settings.json    # Application settings
```

## Technology Stack

- **Framework**: WPF with .NET 10
- **MVVM**: CommunityToolkit.Mvvm
- **Data**: Newtonsoft.Json for persistence
- **Design**: Windows Fluent Design with Acrylic effects

## Project Structure

```
GlassNotes/
├── Models/              # Data models (Note, AppSettings)
├── ViewModels/          # MVVM view models
├── Services/            # Business logic (NoteService, SettingsService)
├── Helpers/             # Utility classes (WindowBlurHelper, ThemeHelper)
├── Resources/
│   ├── Themes/         # Light and Dark theme definitions
│   └── Styles/         # Control styles (buttons, textboxes, etc.)
├── Controls/           # Custom user controls
└── Assets/             # Icons and images
```

## License

This project is open-source and licensed under the [MIT License](LICENSE).

## Contributing

Contributions are welcome! Feel free to submit issues and pull requests.

