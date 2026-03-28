using Terminal.Gui;

namespace ProtoEditorConsoleApp;

/// <summary>
/// Left-panel file explorer that shows only .proto files from a root directory.
/// Renders as a tree view (directories as nodes, .proto files as leaves).
/// </summary>
internal class FileExplorerView : View
{
    private readonly string _rootDirectory;
    private readonly TreeView<FileSystemNode> _treeView;

    /// <summary>Raised when the user selects (Enter / double-click) a .proto file.</summary>
    public event Action<string>? FileSelected;

    public FileExplorerView(string rootDirectory)
    {
        _rootDirectory = rootDirectory;

        _treeView = new TreeView<FileSystemNode>
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        _treeView.TreeBuilder = new FileSystemTreeBuilder();
        _treeView.AspectGetter = node => node.DisplayName;

        // Populate root
        var rootNode = new FileSystemNode(_rootDirectory, isDirectory: true);
        _treeView.AddObject(rootNode);
        _treeView.Expand(rootNode);

        _treeView.SelectionChanged += OnSelectionChanged;
        _treeView.KeyDown += OnKeyDown;

        Add(_treeView);
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs<FileSystemNode> e)
    {
        // Preview on selection change (single click / arrow key)
        if (e.NewValue is { IsDirectory: false })
            FileSelected?.Invoke(e.NewValue.FullPath);
    }

    private void OnKeyDown(object? sender, Key e)
    {
        if (e == Key.Enter)
        {
            var selected = _treeView.SelectedObject;
            if (selected is { IsDirectory: false })
                FileSelected?.Invoke(selected.FullPath);
        }
    }
}

// ── Tree node model ───────────────────────────────────────────────────────────

internal class FileSystemNode
{
    public string FullPath { get; }
    public bool IsDirectory { get; }
    public string DisplayName { get; }

    public FileSystemNode(string fullPath, bool isDirectory)
    {
        FullPath = fullPath;
        IsDirectory = isDirectory;
        DisplayName = isDirectory
            ? $"📁 {Path.GetFileName(fullPath)}"
            : $"  {Path.GetFileName(fullPath)}";
    }
}

internal class FileSystemTreeBuilder : ITreeBuilder<FileSystemNode>
{
    public bool SupportsCanExpand => true;

    public bool CanExpand(FileSystemNode model)
        => model.IsDirectory && HasProtoDescendants(model.FullPath);

    public IEnumerable<FileSystemNode> GetChildren(FileSystemNode model)
    {
        if (!model.IsDirectory)
            yield break;

        // Subdirectories that contain at least one .proto file recursively
        foreach (var dir in Directory.EnumerateDirectories(model.FullPath)
                     .Where(d => HasProtoDescendants(d))
                     .OrderBy(d => d))
            yield return new FileSystemNode(dir, isDirectory: true);

        // Direct .proto files
        foreach (var file in Directory.EnumerateFiles(model.FullPath, "*.proto")
                     .OrderBy(f => f))
            yield return new FileSystemNode(file, isDirectory: false);
    }

    private static bool HasProtoDescendants(string directory)
    {
        try
        {
            return Directory.EnumerateFiles(directory, "*.proto", SearchOption.AllDirectories).Any();
        }
        catch
        {
            return false;
        }
    }
}

