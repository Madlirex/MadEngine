using ImGuiNET;

namespace MadEditor;

public class StatsDrawer : IPanelDrawer
{
    public string Name => "Stats";
    public PanelRegion PanelRegion { get; set; } = PanelRegion.Bottom;
    public void Draw(EditorUIContext context)
    {
        ImGui.Text($"FPS        : {1.0 / context.Window.UpdateTime:F0}");
        ImGui.Text($"Frame time : {context.Window.UpdateTime * 1000.0:F2} ms");

        var pos = context.CameraObject.Transform.Position;
        ImGui.Text($"Camera     : {pos.X:F2}, {pos.Y:F2}, {pos.Z:F2}");
        ImGui.Text($"Viewport   : {context.ViewportSize.X:F0} x {context.ViewportSize.Y:F0}");
    }
}