using System.Numerics;
using ImGuiNET;

namespace MadEditor;

public class PanelArea
{
    public Vector2 Position;
    public Vector2 Size;
    public Vector2 SizePercentage;

    public float Width
    {
        get => Size.X;
        set => Size.X = value;
    }

    public float Height
    {
        get => Size.Y;
        set => Size.Y = value;
    }

    public float WidthPercentage
    {
        get => SizePercentage.X;
        set => SizePercentage.X = value;
    }

    public float HeightPercentage
    {
        get => SizePercentage.Y;
        set => SizePercentage.Y = value;
    }
}

public enum PanelRegion
{
    Left,
    Center,
    Right,
    Bottom
}

public static class PanelLayoutManager
{
    public static IReadOnlyDictionary<PanelRegion, PanelArea> PanelRegionsProperties => _areas;
    
    private static Dictionary<PanelRegion, PanelArea> _areas = new()
    {
        [PanelRegion.Left] = new PanelArea { SizePercentage = new Vector2(0.18f, 1f) },
        [PanelRegion.Right] = new PanelArea { SizePercentage = new Vector2(0.22f, 1f) },
        [PanelRegion.Center] = new PanelArea { SizePercentage = new Vector2(0.6f, 0.8f) },
        [PanelRegion.Bottom] = new PanelArea { SizePercentage = new Vector2(0.6f, 0.2f) },
    };

    public static PanelArea Get(PanelRegion region) => _areas[region];
    
    public static void Update()
    {
        Vector2 screenPos = ImGui.GetMainViewport().WorkPos;
        Vector2 screenSize = ImGui.GetMainViewport().WorkSize;

        foreach (PanelArea panelArea in _areas.Values)
        {
            RecalculateSize(panelArea, screenSize);
        }

        _areas[PanelRegion.Left].Position = screenPos;

        _areas[PanelRegion.Center].Position = _areas[PanelRegion.Left].Position + new Vector2(_areas[PanelRegion.Left].Width, 0f);

        _areas[PanelRegion.Right].Position = _areas[PanelRegion.Center].Position + new Vector2(_areas[PanelRegion.Center].Width, 0f);

        _areas[PanelRegion.Bottom].Position = _areas[PanelRegion.Left].Position + new Vector2(_areas[PanelRegion.Left].Width, _areas[PanelRegion.Center].Height);
    }
    
    private static void RecalculateSize(PanelArea panelArea, Vector2 screenSize)
    {
        panelArea.Width = MathF.Floor(screenSize.X * panelArea.WidthPercentage);
        panelArea.Height = MathF.Floor(screenSize.Y * panelArea.HeightPercentage);
    }
}