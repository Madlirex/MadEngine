using System.Runtime.InteropServices;
using System.Numerics;

namespace ImGuiNET;

public static unsafe class ImGuiInternal
{
    [DllImport("cimgui", EntryPoint = "igStringToHash", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetID(string str_id);

    [DllImport("cimgui", EntryPoint = "igDockBuilderRemoveNode", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DockBuilderRemoveNode(uint node_id);

    [DllImport("cimgui", EntryPoint = "igDockBuilderAddNode", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DockBuilderAddNode(uint node_id, ImGuiDockNodeFlags flags);

    [DllImport("cimgui", EntryPoint = "igDockBuilderSetNodeSize", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DockBuilderSetNodeSize(uint node_id, Vector2 size);

    [DllImport("cimgui", EntryPoint = "igDockBuilderSplitNode", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint DockBuilderSplitNode(uint node_id, ImGuiDir split_dir, float size_ratio_for_node_at_dir, out uint out_id_at_dir, out uint out_id_at_opposite_dir);

    [DllImport("cimgui", EntryPoint = "igDockBuilderFinish", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DockBuilderFinish(uint node_id);
    
    [DllImport("cimgui", EntryPoint = "igDockBuilderDockWindow", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DockBuilderDockWindow(string window_name, uint node_id);
}