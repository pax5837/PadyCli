using Terminal.Gui;

namespace ProtoEditorConsoleApp;

/// <summary>
/// Middle-panel proto file editor.
/// Supports: display, editing, Ctrl+S save, F12 go-to-definition.
/// </summary>
internal class ProtoEditorView : View
{
    private readonly TextView _textView;
    private string? _currentFilePath;
    private bool _isDirty;

    /// <summary>Raised when the user presses F12. The argument is the word under the cursor.</summary>
    public event Action<string>? GoToDefinitionRequested;

    /// <summary>Raised when the user presses Ctrl+S.</summary>
    public event Action? SaveRequested;

    public string? CurrentFilePath => _currentFilePath;

    public ProtoEditorView()
    {
        _textView = new TextView
        {
            X        = 0,
            Y        = 0,
            Width    = Dim.Fill(),
            Height   = Dim.Fill(),
            ReadOnly = false,
            WordWrap = false,
        };

        _textView.ContentsChanged += (_, _) => _isDirty = true;
        _textView.KeyDown         += OnKeyDown;

        Add(_textView);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Load a .proto file into the editor.</summary>
    public void LoadFile(string filePath)
    {
        try
        {
            _textView.Text   = File.ReadAllText(filePath);
            _currentFilePath = filePath;
            _isDirty         = false;
            _textView.MoveHome();
        }
        catch (Exception ex)
        {
            _textView.Text   = $"// Error loading file:\n// {ex.Message}";
            _currentFilePath = null;
        }
    }

    /// <summary>Scroll the view to a specific 0-based line index.</summary>
    public void ScrollToLine(int lineIndex)
    {
        if (lineIndex < 0) return;
        // In Terminal.Gui v2 use CurrentRow/CurrentColumn to position cursor
        _textView.ScrollTo(lineIndex, true);
        SetFocus();
    }

    /// <summary>Save the current content back to disk.</summary>
    public SaveResult Save()
    {
        if (_currentFilePath is null)
            return new SaveResult(false, "No file is currently open.");
        try
        {
            File.WriteAllText(_currentFilePath, _textView.Text.ToString());
            _isDirty = false;
            return new SaveResult(true, null);
        }
        catch (Exception ex)
        {
            return new SaveResult(false, ex.Message);
        }
    }

    public bool IsDirty => _isDirty;

    // ── Keyboard handling ─────────────────────────────────────────────────────

    private void OnKeyDown(object? sender, Key e)
    {
        // F12 → go to definition
        if (e == Key.F12)
        {
            var word = GetWordUnderCursor();
            GoToDefinitionRequested?.Invoke(word);
            e.Handled = true;
            return;
        }

        // Ctrl+S → save  (v2: Key.S.WithCtrl)
        if (e == Key.S.WithCtrl)
        {
            SaveRequested?.Invoke();
            e.Handled = true;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Extract the proto identifier (message/type name) under the text cursor.
    /// Uses TextView.CurrentRow / CurrentColumn (Terminal.Gui v2 API).
    /// </summary>
    private string GetWordUnderCursor()
    {
        var text = _textView.Text?.ToString();
        if (string.IsNullOrEmpty(text)) return string.Empty;

        // v2: CurrentRow and CurrentColumn are plain int properties
        var row = _textView.CurrentRow;
        var col = _textView.CurrentColumn;

        var lines = text.Split('\n');
        if (row >= lines.Length) return string.Empty;

        var line = lines[row];
        if (line.Length == 0) return string.Empty;

        col = Math.Clamp(col, 0, line.Length - 1);

        // Expand left
        var start = col;
        while (start > 0 && IsIdentifierChar(line[start - 1]))
            start--;

        // Expand right
        var end = col;
        while (end < line.Length - 1 && IsIdentifierChar(line[end + 1]))
            end++;

        return line.Substring(start, end - start + 1).Trim();
    }

    private static bool IsIdentifierChar(char c)
        => char.IsLetterOrDigit(c) || c == '_' || c == '.';
}

// ── Result type ───────────────────────────────────────────────────────────────

internal record SaveResult(bool Success, string? ErrorMessage);

