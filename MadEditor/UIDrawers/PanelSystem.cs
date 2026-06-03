using System.Numerics;
using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public static class PanelSystem
{
    private static readonly ImGuiWindowFlags FixedPanel =
        ImGuiWindowFlags.NoMove        |
        ImGuiWindowFlags.NoResize      |
        ImGuiWindowFlags.NoCollapse    |
        ImGuiWindowFlags.NoBringToFrontOnFocus;
    
    private static bool _initialized;
    
    public static IReadOnlyList<Type> PanelDrawers => _panelDrawers;
    private static List<Type> _panelDrawers = [];

    public static IReadOnlyList<IPanelDrawer> Panels => _panels;
    private static List<IPanelDrawer> _panels = [];

    public static void Initialize()
    {
        if (_initialized)
            return;
        
        Type[] panels = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IPanelDrawer).IsAssignableFrom(p)).ToArray();

        foreach (Type panelDrawer in panels)
        {
            Register(panelDrawer);
        }

        _initialized = true;
    }
    
    public static void Register(Type panelDrawer)
    {
        if (!typeof(IPanelDrawer).IsAssignableFrom(panelDrawer))
            throw new InvalidOperationException("Not a valid panel");

        if (_panelDrawers.Contains(panelDrawer))
            throw new InvalidOperationException("PanelDrawer already registered");

        _panelDrawers.Add(panelDrawer);
    }

    public static void Unregister(Type panelDrawer)
    {
        if (!typeof(IPanelDrawer).IsAssignableFrom(panelDrawer))
            throw new InvalidOperationException("Not a valid panel");

        if (!_panelDrawers.Remove(panelDrawer))
            throw new InvalidOperationException("PanelDrawer not registered");
    }

    public static void CreatePanel<T>() where T : IPanelDrawer, new()
    {
        _panels.Add(new T());
    }

    public static void DeletePanel(IPanelDrawer panelDrawer)
    {
        if(!_panels.Remove(panelDrawer))
            throw new InvalidOperationException("PanelDrawer not instantiated");
    }

    public static void Draw(EditorUIContext context)
    {
        foreach (IPanelDrawer panelDrawer in _panels)
        {
            if(panelDrawer is ViewportDrawer)
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            
            PanelArea panelArea = PanelLayoutManager.Get(panelDrawer.PanelRegion);
            ImGui.SetNextWindowPos(panelArea.Position);
            ImGui.SetNextWindowSize(panelArea.Size);
            
            ImGui.Begin(panelDrawer.Name, FixedPanel);
            if(panelDrawer is ViewportDrawer)
                ImGui.PopStyleVar();
            panelDrawer.Draw(context);
            ImGui.End();
        }
    }
}