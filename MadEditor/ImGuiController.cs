using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace MadEditor;

public class ImGuiController : IDisposable
{
    private int _vao;
    private int _vbo;
    private int _ebo;
    private int _fontTexture;
    private int _shader;
    private int _uniformProjection;
    
    private int _windowWidth;
    private int _windowHeight;
    private bool _frameBegun;
    private readonly List<char> _pressedChars = new();
    
    private const int VertexSize = 20;
    
    private const string VertSrc = @"
#version 330 core
layout (location = 0) in vec2 Position;
layout (location = 1) in vec2 UV;
layout (location = 2) in vec4 Color;

uniform mat4 projection;
out vec2 Frag_UV;
out vec4 Frag_Color;

void main()
{
    Frag_UV    = UV;
    Frag_Color = Color;
    gl_Position = projection * vec4(Position, 0, 1);
}";

    private const string FragSrc = @"
#version 330 core
in vec2  Frag_UV;
in vec4  Frag_Color;
uniform sampler2D Texture;
out vec4 Out_Color;
void main()
{
    Out_Color = Frag_Color * texture(Texture, Frag_UV.st);
}";

    public ImGuiController(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;

        ImGui.CreateContext();
        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.AddFontDefault();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        CreateDeviceObjects();
        SetKeyMappings();
        SetPerFrameImGuiData(1f / 60f);
        ImGui.NewFrame();
        _frameBegun = true;
    }
    
    public void Update(GameWindow wnd, float deltaSeconds)
    {
        if (_frameBegun)
            ImGui.Render();

        SetPerFrameImGuiData(deltaSeconds);
        UpdateImGuiInput(wnd);

        _frameBegun = true;
        ImGui.NewFrame();
    }

    public void Render()
    {
        if (!_frameBegun) return;
        _frameBegun = false;
        ImGui.Render();
        RenderImDrawData(ImGui.GetDrawData());
    }
    
    public void PressChar(char c) => _pressedChars.Add(c);
    
    public void Resize(int width, int height)
    {
        _windowWidth  = width;
        _windowHeight = height;
    }
    
    private void CreateDeviceObjects()
    {
        int vert = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vert, VertSrc);
        GL.CompileShader(vert);
        CheckShader(vert, "ImGui vertex");

        int frag = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(frag, FragSrc);
        GL.CompileShader(frag);
        CheckShader(frag, "ImGui fragment");

        _shader = GL.CreateProgram();
        GL.AttachShader(_shader, vert);
        GL.AttachShader(_shader, frag);
        GL.LinkProgram(_shader);
        CheckProgram(_shader, "ImGui");

        GL.DetachShader(_shader, vert);
        GL.DetachShader(_shader, frag);
        GL.DeleteShader(vert);
        GL.DeleteShader(frag);

        _uniformProjection = GL.GetUniformLocation(_shader, "projection");
        int texSlot = GL.GetUniformLocation(_shader, "Texture");
        GL.UseProgram(_shader);
        GL.Uniform1(texSlot, 0);
        
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, VertexSize, 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexSize, 8);

        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, VertexSize, 16);

        GL.BindVertexArray(0);
        
        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int fw, out int fh, out _);

        _fontTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                      fw, fh, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

        io.Fonts.SetTexID(_fontTexture);
        io.Fonts.ClearTexData();
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }
    

    private void RenderImDrawData(ImDrawDataPtr drawData)
    {
        if (drawData.CmdListsCount == 0) return;
        
        GL.GetInteger(GetPName.ActiveTexture, out int prevActiveTexture);
        GL.GetInteger(GetPName.CurrentProgram, out int prevProgram);
        GL.GetInteger(GetPName.TextureBinding2D, out int prevTexture);
        GL.GetInteger(GetPName.ArrayBufferBinding, out int prevVbo);
        GL.GetInteger(GetPName.VertexArrayBinding, out int prevVao);
        GL.GetInteger(GetPName.BlendSrcRgb, out int prevBlendSrcRgb);
        GL.GetInteger(GetPName.BlendDstRgb, out int prevBlendDstRgb);
        GL.GetInteger(GetPName.BlendSrcAlpha, out int prevBlendSrcAlpha);
        GL.GetInteger(GetPName.BlendDstAlpha, out int prevBlendDstAlpha);
        GL.GetInteger(GetPName.BlendEquationRgb, out int prevBlendEqRgb);
        GL.GetInteger(GetPName.BlendEquationAlpha, out int prevBlendEqAlpha);
        bool prevBlend = GL.IsEnabled(EnableCap.Blend);
        bool prevCullFace = GL.IsEnabled(EnableCap.CullFace);
        bool prevDepthTest = GL.IsEnabled(EnableCap.DepthTest);
        bool prevScissorTest = GL.IsEnabled(EnableCap.ScissorTest);
        
        GL.Enable(EnableCap.Blend);
        GL.BlendEquation(BlendEquationMode.FuncAdd);
        GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.ScissorTest);
        GL.ActiveTexture(TextureUnit.Texture0);
        
        float l = drawData.DisplayPos.X;
        float r = drawData.DisplayPos.X + drawData.DisplaySize.X;
        float t = drawData.DisplayPos.Y;
        float b = drawData.DisplayPos.Y + drawData.DisplaySize.Y;

        Matrix4 proj = new Matrix4(
             2f/(r-l),    0,           0, 0,
             0,           2f/(t-b),    0, 0,
             0,           0,          -1, 0,
            (r+l)/(l-r), (t+b)/(b-t), 0, 1);

        GL.UseProgram(_shader);
        GL.UniformMatrix4(_uniformProjection, false, ref proj);
        GL.BindVertexArray(_vao);

        Vector2 clipOff = drawData.DisplayPos;
        Vector2 clipScale = drawData.FramebufferScale;

        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[n];

            int vtxBytes = cmdList.VtxBuffer.Size * VertexSize;
            int idxBytes = cmdList.IdxBuffer.Size * sizeof(ushort);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vtxBytes, cmdList.VtxBuffer.Data, BufferUsageHint.StreamDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, idxBytes, cmdList.IdxBuffer.Data, BufferUsageHint.StreamDraw);

            for (int cmdIdx = 0; cmdIdx < cmdList.CmdBuffer.Size; cmdIdx++)
            {
                ImDrawCmdPtr cmd = cmdList.CmdBuffer[cmdIdx];

                if (cmd.UserCallback != IntPtr.Zero)
                    continue;

                Vector4 cr = cmd.ClipRect;
                float cx = (cr.X - clipOff.X) * clipScale.X;
                float cy = (cr.Y - clipOff.Y) * clipScale.Y;
                float cw = (cr.Z - clipOff.X) * clipScale.X;
                float ch = (cr.W - clipOff.Y) * clipScale.Y;
                if (cw <= cx || ch <= cy) continue;

                GL.Scissor((int)cx, _windowHeight - (int)ch, (int)(cw - cx), (int)(ch - cy));
                GL.BindTexture(TextureTarget.Texture2D, (int)cmd.TextureId);
                GL.DrawElementsBaseVertex(
                    PrimitiveType.Triangles,
                    (int)cmd.ElemCount,
                    DrawElementsType.UnsignedShort,
                    (IntPtr)(cmd.IdxOffset * sizeof(ushort)),
                    (int)cmd.VtxOffset);
            }
        }
        
        GL.UseProgram(prevProgram);
        GL.BindTexture(TextureTarget.Texture2D, prevTexture);
        GL.ActiveTexture((TextureUnit)prevActiveTexture);
        GL.BindVertexArray(prevVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, prevVbo);
        GL.BlendEquationSeparate((BlendEquationMode)prevBlendEqRgb, (BlendEquationMode)prevBlendEqAlpha);
        GL.BlendFuncSeparate((BlendingFactorSrc)prevBlendSrcRgb, (BlendingFactorDest)prevBlendDstRgb,
                             (BlendingFactorSrc)prevBlendSrcAlpha, (BlendingFactorDest)prevBlendDstAlpha);
        if (prevBlend) GL.Enable(EnableCap.Blend); else GL.Disable(EnableCap.Blend);
        if (prevCullFace) GL.Enable(EnableCap.CullFace); else GL.Disable(EnableCap.CullFace);
        if (prevDepthTest) GL.Enable(EnableCap.DepthTest); else GL.Disable(EnableCap.DepthTest);
        if (prevScissorTest) GL.Enable(EnableCap.ScissorTest); else GL.Disable(EnableCap.ScissorTest);
    }

    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(_windowWidth, _windowHeight);
        if (_windowWidth > 0 && _windowHeight > 0)
            io.DisplayFramebufferScale = new Vector2(1f, 1f);
        io.DeltaTime = deltaSeconds;
    }

    // Maps every GLFW Keys value that has a direct ImGuiKey counterpart.
    private static readonly (Keys glfw, ImGuiKey imgui)[] KeyMap =
    [
        (Keys.Tab,          ImGuiKey.Tab),
        (Keys.Left,         ImGuiKey.LeftArrow),
        (Keys.Right,        ImGuiKey.RightArrow),
        (Keys.Up,           ImGuiKey.UpArrow),
        (Keys.Down,         ImGuiKey.DownArrow),
        (Keys.PageUp,       ImGuiKey.PageUp),
        (Keys.PageDown,     ImGuiKey.PageDown),
        (Keys.Home,         ImGuiKey.Home),
        (Keys.End,          ImGuiKey.End),
        (Keys.Insert,       ImGuiKey.Insert),
        (Keys.Delete,       ImGuiKey.Delete),
        (Keys.Backspace,    ImGuiKey.Backspace),
        (Keys.Space,        ImGuiKey.Space),
        (Keys.Enter,        ImGuiKey.Enter),
        (Keys.Escape,       ImGuiKey.Escape),
        (Keys.LeftControl,  ImGuiKey.LeftCtrl),
        (Keys.LeftShift,    ImGuiKey.LeftShift),
        (Keys.LeftAlt,      ImGuiKey.LeftAlt),
        (Keys.LeftSuper,    ImGuiKey.LeftSuper),
        (Keys.RightControl, ImGuiKey.RightCtrl),
        (Keys.RightShift,   ImGuiKey.RightShift),
        (Keys.RightAlt,     ImGuiKey.RightAlt),
        (Keys.RightSuper,   ImGuiKey.RightSuper),
        (Keys.A,  ImGuiKey.A), (Keys.B, ImGuiKey.B), (Keys.C, ImGuiKey.C),
        (Keys.D,  ImGuiKey.D), (Keys.E, ImGuiKey.E), (Keys.F, ImGuiKey.F),
        (Keys.G,  ImGuiKey.G), (Keys.H, ImGuiKey.H), (Keys.I, ImGuiKey.I),
        (Keys.J,  ImGuiKey.J), (Keys.K, ImGuiKey.K), (Keys.L, ImGuiKey.L),
        (Keys.M,  ImGuiKey.M), (Keys.N, ImGuiKey.N), (Keys.O, ImGuiKey.O),
        (Keys.P,  ImGuiKey.P), (Keys.Q, ImGuiKey.Q), (Keys.R, ImGuiKey.R),
        (Keys.S,  ImGuiKey.S), (Keys.T, ImGuiKey.T), (Keys.U, ImGuiKey.U),
        (Keys.V,  ImGuiKey.V), (Keys.W, ImGuiKey.W), (Keys.X, ImGuiKey.X),
        (Keys.Y,  ImGuiKey.Y), (Keys.Z, ImGuiKey.Z),
        (Keys.D0, ImGuiKey._0), (Keys.D1, ImGuiKey._1), (Keys.D2, ImGuiKey._2),
        (Keys.D3, ImGuiKey._3), (Keys.D4, ImGuiKey._4), (Keys.D5, ImGuiKey._5),
        (Keys.D6, ImGuiKey._6), (Keys.D7, ImGuiKey._7), (Keys.D8, ImGuiKey._8),
        (Keys.D9, ImGuiKey._9),
        (Keys.F1,  ImGuiKey.F1),  (Keys.F2,  ImGuiKey.F2),  (Keys.F3,  ImGuiKey.F3),
        (Keys.F4,  ImGuiKey.F4),  (Keys.F5,  ImGuiKey.F5),  (Keys.F6,  ImGuiKey.F6),
        (Keys.F7,  ImGuiKey.F7),  (Keys.F8,  ImGuiKey.F8),  (Keys.F9,  ImGuiKey.F9),
        (Keys.F10, ImGuiKey.F10), (Keys.F11, ImGuiKey.F11), (Keys.F12, ImGuiKey.F12)
    ];

    private void UpdateImGuiInput(GameWindow wnd)
    {
        ImGuiIOPtr io = ImGui.GetIO();

        MouseState mouse = wnd.MouseState;
        KeyboardState kb = wnd.KeyboardState;
        
        io.AddMouseButtonEvent(0, mouse.IsButtonDown(MouseButton.Left));
        io.AddMouseButtonEvent(1, mouse.IsButtonDown(MouseButton.Right));
        io.AddMouseButtonEvent(2, mouse.IsButtonDown(MouseButton.Middle));
        io.AddMousePosEvent(mouse.X, mouse.Y);
        io.AddMouseWheelEvent(mouse.ScrollDelta.X, mouse.ScrollDelta.Y);

        io.AddKeyEvent(ImGuiKey.ModCtrl, kb.IsKeyDown(Keys.LeftControl) || kb.IsKeyDown(Keys.RightControl));
        io.AddKeyEvent(ImGuiKey.ModShift, kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift));
        io.AddKeyEvent(ImGuiKey.ModAlt, kb.IsKeyDown(Keys.LeftAlt) || kb.IsKeyDown(Keys.RightAlt));
        io.AddKeyEvent(ImGuiKey.ModSuper, kb.IsKeyDown(Keys.LeftSuper) || kb.IsKeyDown(Keys.RightSuper));
        
        foreach ((Keys glfw, ImGuiKey imgui) in KeyMap)
            io.AddKeyEvent(imgui, kb.IsKeyDown(glfw));
        
        foreach (char c in _pressedChars)
            io.AddInputCharacter(c);
        _pressedChars.Clear();
    }
    
    private static void SetKeyMappings() { }
    
    private static void CheckShader(int handle, string name)
    {
        GL.GetShader(handle, ShaderParameter.CompileStatus, out int ok);
        if (ok == 0)
            throw new Exception($"ImGui {name} shader error:\n{GL.GetShaderInfoLog(handle)}");
    }

    private static void CheckProgram(int handle, string name)
    {
        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int ok);
        if (ok == 0)
            throw new Exception($"ImGui {name} program link error:\n{GL.GetProgramInfoLog(handle)}");
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);
        GL.DeleteTexture(_fontTexture);
        GL.DeleteProgram(_shader);
        ImGui.DestroyContext();
    }
}