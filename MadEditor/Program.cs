using NativeFileDialogSharp;

namespace MadEditor;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            DialogResult result = Dialog.FolderPicker();

            if (result.IsCancelled) return;
            if (!result.IsOk) continue;
            AssetManager.LoadProject(result.Path); 
            break;
        }
        
        using (EditorWindow game = new EditorWindow(1200, 800, "Mad Engine"))
        {
            game.Run();
        }
    }
}

