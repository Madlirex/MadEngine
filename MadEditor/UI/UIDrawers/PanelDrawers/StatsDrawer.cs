using ImGuiNET;
using MadEngine.Core;
using OpenTK.Mathematics;

namespace MadEditor;

[CustomName("Statistics")]
public class StatsDrawer : PanelDrawer
{
    public override PanelRegion PanelRegion { get; set; } = PanelRegion.Bottom;
    public double LowestFrameTime = double.MaxValue;
    public override void Draw(EditorUIContext context)
    {
        double currentFrameTime = context.Window.UpdateTime;

        // Prevent checking uninitialized or zero-value frames
        if (currentFrameTime > 0)
        {
            // Find the absolute lowest frame time (which is the highest FPS)
            if (currentFrameTime < LowestFrameTime)
            {
                LowestFrameTime = currentFrameTime;
            }
        }

        // Safely calculate your absolute highest FPS peak
        double peakFps = LowestFrameTime > 0 ? (1.0 / LowestFrameTime) : 0;

        ImGui.Text($"Peak FPS   : {peakFps:F0}");
        ImGui.Text($"Min Frame  : {LowestFrameTime * 1000.0:F2} ms");
        ImGui.Text($"FPS        : {1.0 / currentFrameTime:F0}");

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