using Terminal.Gui;

namespace ProtoEditorConsoleApp;

/// <summary>
/// The main TUI application window.
/// Layout: [FileExplorer (left, ~28%)] | [ProtoEditor (middle/right, remaining)]
/// Keys: F12 = Go to Definition | Ctrl+S = Save | Ctrl+Q = Quit
/// </summary>
internal class ProtoEditorApp : Toplevel
{
    private readonly string _rootDirectory;
    private FileExplorerView _fileExplorer = null!;
    private ProtoEditorView _editor = null!;
    private readonly Label _statusLabel;

    public ProtoEditorApp(string rootDirectory)
    {
        _rootDirectory = rootDirectory;

        var leftPanel  = BuildLeftPanel();
        var midPanel   = BuildMiddlePanel();
        _statusLabel   = BuildStatusLabel();

        Add(leftPanel, midPanel, _statusLabel);
    }

    // ── Panel builders ────────────────────────────────────────────────────────

    private FrameView BuildLeftPanel()
    {
        var panel = new FrameView
        {
            Title  = "Proto Files",
            X      = 0,
            Y      = 0,
            Width  = Dim.Percent(28),
            Height = Dim.Fill(1),
        };

        _fileExplorer = new FileExplorerView(_rootDirectory);
        _fileExplorer.FileSelected += OnFileSelected;
        panel.Add(_fileExplorer);
        return panel;
    }

    private FrameView BuildMiddlePanel()
    {
        var panel = new FrameView
        {
            Title  = "Editor",
            X      = Pos.Percent(28),
            Y      = 0,
            Width  = Dim.Fill(),
            Height = Dim.Fill(1),
        };

        _editor = new ProtoEditorView();
        _editor.GoToDefinitionRequested += OnGoToDefinitionRequested;
        _editor.SaveRequested           += OnSaveRequested;
        panel.Add(_editor);
        return panel;
    }

    private static Label BuildStatusLabel()
        => new()
        {
            Text  = "F12: Go to Def | ^S: Save | ^Q: Quit",
            X     = 0,
            Y     = Pos.AnchorEnd(1),
            Width = Dim.Fill(),
        };

    // ── Event handlers ────────────────────────────────────────────────────────

    private void OnFileSelected(string filePath)
    {
        _editor.LoadFile(filePath);
        _statusLabel.Text = $"F12: Go to Def | ^S: Save | ^Q: Quit  [{Path.GetFileName(filePath)}]";
    }

    private void OnGoToDefinitionRequested(string word) => HandleGoToDefinition(word);

    private void HandleGoToDefinition(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return;
        var location = ProtoDefinitionFinder.Find(_rootDirectory, word);
        if (location is null) ShowNotFound(word);
        else NavigateTo(location);
    }

    private void NavigateTo(ProtoDefinitionLocation location)
    {
        if (_editor.CurrentFilePath != location.FilePath)
            _editor.LoadFile(location.FilePath);
        _editor.ScrollToLine(location.LineIndex);
    }

    private static void ShowNotFound(string typeName)
        => MessageBox.Query("Go to Definition", $"No definition found for '{typeName}'.", "Ok");

    private void OnSaveRequested()
    {
        var result = _editor.Save();
        if (!result.Success)
            MessageBox.ErrorQuery("Save Error", result.ErrorMessage ?? "Unknown error.", "Ok");
    }
}
