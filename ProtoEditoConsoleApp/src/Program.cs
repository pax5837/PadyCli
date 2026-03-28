using ProtoEditorConsoleApp;
using Terminal.Gui;

// Accept optional directory argument
var rootDirectory = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

Application.Init();
Application.Run(new ProtoEditorApp(rootDirectory));
Application.Shutdown();

