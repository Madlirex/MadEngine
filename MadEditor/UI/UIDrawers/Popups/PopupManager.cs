namespace MadEditor;

public static class PopupManager
{
    public static IReadOnlyList<Popup> Popups => PopupsInternal;
    private static readonly List<Popup> PopupsInternal = [];
    
    public static void Draw(EditorUIContext context)
    {
        foreach (var popup in PopupsInternal.ToArray())
        {
            popup.Draw(context);
        }
    }

    public static void Add(Popup popup)
    {
        PopupsInternal.Add(popup);
    }

    public static void Remove(Popup popup)
    {
        PopupsInternal.Remove(popup);
    }
}