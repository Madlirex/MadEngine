using System.Numerics;
 using ImGuiNET;
 using MadEngine.Core;
 using OpenTK.Windowing.Common;
 
 namespace MadEditor;
 
 public class ViewportDrawer : IPanelDrawer
 {
     public string Name => "Viewport";
     public PanelRegion PanelRegion { get; set; } = PanelRegion.Center;
     public void Draw(EditorUIContext context)
     {
         Vector2 availableSpace = ImGui.GetContentRegionAvail();
        
         float availableW = availableSpace.X;
         float availableH = availableSpace.Y;
         
         if (availableW > 1 && availableH > 1)
         {
             if (availableW != context.ViewportSize.X || availableH != context.ViewportSize.Y)
             {
                 context.ViewportSize = new Vector2(availableW, availableH);
                 context.SceneFbo.Resize((int)availableW, (int)availableH);
                 
                 Camera cam = context.CameraObject.GetComponent<Camera>()!;
                 cam.Width = (int)availableW;
                 cam.Height = (int)availableH;
             }
         }
         
         if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
             context.Window!.CursorState = CursorState.Grabbed;
         
         ImGui.Image(context.SceneFbo.ColorTexture, context.ViewportSize, new Vector2(0, 1), new Vector2(1, 0));
         
         if (context.Window!.CursorState == CursorState.Normal)
         {
             ImGui.SetCursorPos(new Vector2(8, ImGui.GetFrameHeight() + 4));
             ImGui.TextDisabled("Right-click + WASD to fly  |  Esc to release");
         }
     }
 }