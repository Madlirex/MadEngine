using ImGuiNET;
using MadEngine.Core;
using OpenTK.Mathematics;

namespace MadEditor;

[CustomName("Statistics")]
public class StatsDrawer : PanelDrawer
{
    public override PanelRegion PanelRegion { get; set; } = PanelRegion.Bottom;
    public override void Draw(EditorUIContext context)
    {
        ImGui.Text($"FPS        : {1.0 / context.Window.UpdateTime:F0}");
        ImGui.Text($"Frame time : {context.Window.UpdateTime * 1000.0:F2} ms");

        Vector3? pos = context.ActiveViewport?.CameraObject.Transform.Position;
        
        string cameraText = pos != null ? 
            $"{pos.Value.X:F2}, {pos.Value.Y:F2}, {pos.Value.Z:F2}" : 
            "None Selected";
        string viewportText = context.ActiveViewport != null ? 
            $"{context.ActiveViewport.Size.X:F0} x {context.ActiveViewport.Size.Y:F0}" : 
            "None Selected";
        ImGui.Text($"Camera     : {cameraText}");
        ImGui.Text($"Viewport   : {viewportText}");
    }
}