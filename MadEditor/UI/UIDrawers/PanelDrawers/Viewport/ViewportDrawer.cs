using System.Numerics;
 using ImGuiNET;
 using MadEngine.Core;
 using OpenTK.Windowing.Common;
 
 namespace MadEditor;
 
 [CustomName("Scene View")]
 public class ViewportDrawer : PanelDrawer
 {
     public override PanelRegion PanelRegion { get; set; } = PanelRegion.Center;
     public override void Draw(EditorUIContext context)
     {
         string panelTitle = ToString();
         ViewportContext viewportContext = context.GetOrCreateViewport(panelTitle);
         
         Vector2 availableSpace = ImGui.GetContentRegionAvail();
        
         float availableW = availableSpace.X;
         float availableH = availableSpace.Y;
         
         if (availableW > 1 && availableH > 1)
         {
             if (availableW != viewportContext.Size.X || availableH != viewportContext.Size.Y)
             {
                 viewportContext.Size = new Vector2(availableW, availableH);
                 viewportContext.Framebuffer.Resize((int)availableW, (int)availableH);
                 
                 Camera cam = viewportContext.CameraObject.GetComponent<Camera>()!;
                 cam.Width = (int)availableW;
                 cam.Height = (int)availableH;
             }
         }
         
         if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
         {
             context.Window!.CursorState = CursorState.Grabbed;
             context.ActiveViewport = viewportContext;
         }
         
         ImGui.Image(viewportContext.Framebuffer.ColorTexture, viewportContext.Size, new Vector2(0, 1), new Vector2(1, 0));
         
         if (context.Window!.CursorState == CursorState.Normal)
         {
             ImGui.SetCursorPos(new Vector2(8, ImGui.GetFrameHeight() + 4));
             ImGui.TextDisabled("Right-click + WASD to fly  |  Esc to release");
         }
     }
 }