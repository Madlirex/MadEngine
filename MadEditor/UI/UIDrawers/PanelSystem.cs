using System.Numerics;
using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public static class PanelSystem
{
    private static PanelSystemEngine Instance => RegistryBootstrapper.Get<PanelSystemEngine>();
    
    public static IReadOnlyList<Type> PanelDrawers => Instance.PanelDrawers;
    public static IReadOnlyList<PanelDrawer> Panels => Instance.Panels;
    
    public static void Initialize()
    {
        Instance.CreatePanel<HierarchyDrawer>();
        Instance.CreatePanel<InspectorPanelDrawer>();
        Instance.CreatePanel<ViewportDrawer>();
        Instance.CreatePanel<StatsDrawer>();
    }
    
    public static void Register(Type panelDrawer) => Instance.Register(panelDrawer);
    public static void Unregister(Type panelDrawer) => Instance.Unregister(panelDrawer);
    public static void CreatePanel<T>() where T : PanelDrawer, new() => Instance.CreatePanel<T>();
    public static void AddPanel(PanelDrawer panelDrawer) => Instance.AddPanel(panelDrawer);
    public static void DeletePanel(PanelDrawer panelDrawer) => Instance.DeletePanel(panelDrawer);
    public static void Draw(EditorUIContext context) => Instance.Draw(context);
}

internal class PanelSystemEngine : Registry
{
    private readonly ImGuiWindowFlags FixedPanel =
        ImGuiWindowFlags.NoMove        |
        ImGuiWindowFlags.NoResize      |
        ImGuiWindowFlags.NoCollapse    |
        ImGuiWindowFlags.NoBringToFrontOnFocus;
    
    public IReadOnlyList<Type> PanelDrawers => _panelDrawers;
    private List<Type> _panelDrawers = [];

    public IReadOnlyList<PanelDrawer> Panels => _panels;
    private List<PanelDrawer> _panels = [];

    public override void Initialize()
    {
        _panelDrawers.Clear();
        
        Type[] panels = ScriptDomain.GetTypesImplementing(typeof(PanelDrawer));
        
        foreach (Type panelDrawer in panels)
        {
            Register(panelDrawer);
        }
    }
    
    public void Register(Type panelDrawer)
    {
        if (!typeof(PanelDrawer).IsAssignableFrom(panelDrawer))
            throw new InvalidOperationException("Not a valid panel");

        if (_panelDrawers.Contains(panelDrawer))
            throw new InvalidOperationException("PanelDrawer already registered");

        _panelDrawers.Add(panelDrawer);
    }

    public void Unregister(Type panelDrawer)
    {
        if (!typeof(PanelDrawer).IsAssignableFrom(panelDrawer))
            throw new InvalidOperationException("Not a valid panel");

        if (!_panelDrawers.Remove(panelDrawer))
            throw new InvalidOperationException("PanelDrawer not registered");
    }

    public void CreatePanel<T>() where T : PanelDrawer, new()
    {
        AddPanel(new T());
    }

    public void AddPanel(PanelDrawer panelDrawer)
    {
        _panels.Add(panelDrawer);
        PanelLayoutManager.AddPanel(panelDrawer);
    }

    public void DeletePanel(PanelDrawer panelDrawer)
    {
        if(!_panels.Remove(panelDrawer))
            throw new InvalidOperationException("PanelDrawer not instantiated");
        PanelLayoutManager.DeletePanel(panelDrawer);
    }

    public void Draw(EditorUIContext context)
    {
        uint mainDockSpaceId = PanelLayoutManager.DrawMainDockSpace();

        foreach (PanelDrawer panelDrawer in _panels)
        {
            if (panelDrawer.PanelRegion != PanelRegion.Floating)
            {
                if (panelDrawer is ViewportDrawer)
                    ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
                
                ImGui.Begin(panelDrawer.ToString(), ImGuiWindowFlags.NoCollapse);

                if (panelDrawer is ViewportDrawer)
                    ImGui.PopStyleVar();
            }
            else
            {
                bool openStateCheck = true;
                
                ImGui.SetNextWindowSize(panelDrawer.MinSize, ImGuiCond.FirstUseEver);
                
                ImGui.Begin(panelDrawer.ToString(), ref openStateCheck, ImGuiWindowFlags.None);
                if (!openStateCheck)
                {
                    context.EnqueueCommand(new ClosePanelCommand(panelDrawer));
                    ImGui.End();
                    continue;
                }
            }

            panelDrawer.Draw(context);
            ImGui.End();
        }
    }
}