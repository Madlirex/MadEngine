using MadEditor;
using MadEngine;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

public class EditorTestComp : Component
{
	public Material? Material;
	public Mesh? Mesh;
	public bool Wait = true;
	
	public int Count = 0;
	public int SpawnCount = 1;
	private bool _spawn = true;
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