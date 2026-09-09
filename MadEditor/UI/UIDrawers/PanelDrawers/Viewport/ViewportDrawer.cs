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
         viewportContext.EditMode = true;
         
         Vector2 availableSpace = ImGui.GetContentRegionAvail();

         float availableW = availableSpace.X;
         float availableH = availableSpace.Y;
         
         if (availableW > 1 && availableH > 1)
         {
             if (availableW != viewportContext.Size.X || availableH != viewportContext.Size.Y)
             {
                 viewportContext.Size = new Vector2(availableW, availableH);
                 viewportContext.Framebuffer.Resize((int)availableW, (int)availableH);
                 
                 Camera cam = viewportContext.CameraComponent;
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

         RenderFloatingToolbar(context);
         
         if (context.Window!.CursorState != CursorState.Normal) return;
         ImGui.SetCursorPos(new Vector2(8, ImGui.GetFrameHeight() + 4));
         ImGui.TextDisabled("Right-click + WASD to fly  |  Esc to release");
    }

    private void RenderFloatingToolbar(EditorUIContext context)
    {
        Vector2 windowPos = ImGui.GetWindowPos();
        
        float paddingY = ImGui.GetFrameHeight() + 10.0f;
        float centerX = windowPos.X + (ImGui.GetWindowWidth() * 0.5f) - 45.0f;
        
        ImGui.SetCursorScreenPos(new Vector2(centerX, windowPos.Y + paddingY));
        
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.15f, 0.15f, 0.15f, 0.8f));
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 6.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(6, 4));
        
        var childFlags = ImGuiChildFlags.Borders | ImGuiChildFlags.AlwaysUseWindowPadding;
        if (ImGui.BeginChild("##ViewportToolbar", new Vector2(90, 32), childFlags))
        {
            if (context.IsPlaying)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.2f, 1.0f, 0.2f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 0.1f, 0.1f, 1.0f)); 
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.15f, 0.15f, 0.15f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            }
            
            if (ImGui.Button(" > ", new Vector2(36, 24)))
            {
                context.EnqueueCommand(new EnterPlaymodeCommand());
            }

            if (context.IsPlaying)
            {
                ImGui.PopStyleColor(4);
            }

            ImGui.SameLine();
            
            if (ImGui.Button("||", new Vector2(36, 24)))
            {
                context.EnqueueCommand(new ExitPlaymodeCommand());
            }
        }
        ImGui.EndChild();
        
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor();
    }
}