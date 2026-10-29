using MadEditor;
using MadEngine;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;
using NVector2 = System.Numerics.Vector2;
using OpenTK.Mathematics;
using ImGuiNET;

[CustomFieldDrawer(typeof(Vector2))]
public class Vector2Drawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        Vector2 otkValue = (Vector2)member.GetValue(target)!;
	NVector2 value = new NVector2(otkValue.X, otkValue.Y);

        if (ImGui.DragFloat2(member.GetCustomName(), ref value))
        {
            member.SetValue(target, new Vector2(value.X, value.Y));
        }
    }
}

public class EditorTestComp : Component
{
	public Material? Material;
	public Mesh? Mesh;
	public bool Wait = true;
	
	public int Count = 0;
	public int SpawnCount = 1;
	private bool _spawn = true;
	public MeshRenderer Something;
	public Light Second;
	[ShowInInspector] public bool Spawn { get => _spawn;
		set => SpawnCubes();
	}

    public override void Awake()
    {
	    Debug.Log("Awake");
    }

    public override void Start()
    {
	    Debug.Log("Start");
    }

    public override void Update(float deltaTime)
    {
	    GameObject.Transform.Position.X += deltaTime;
    }

    public override void EditorStart()
    {
	    Debug.Log("EditorTestComp Start");
    }

    public void SpawnCubes()
    {
	    if ((Mesh == null || Material == null) && Wait)
	    {
		    return;
	    }
	
	    for(int i = 0; i < SpawnCount; i++)
	    {
		    GameObject obj = new GameObject();
		    MeshRenderer renderer = new MeshRenderer();
		    renderer.Mesh = Mesh;
		    renderer.Material = Material;
		    obj.AddComponent(renderer);
		    SceneManager.ActiveScene.Add(obj);
	    }

	    Count += SpawnCount;
    }

    public override void EditorUpdate(float deltaTime)
    {
	    
    }
}