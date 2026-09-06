using MadEditor;
using MadEngine;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

public enum Test
{
	Value1,
	Value2,
	_ef,
	DzigaMore
}

public class EditorTestComp : Component
{
    public int Count = 0;
    public Mesh? mesh = null;
    public Material? material = null;
    public Test test;

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

    public override void EditorUpdate(float deltaTime)
    {
	   
	    Debug.LogWarning(test.ToString());
	    
	    if (mesh == null || material == null)
	    {
		    return;
	    }
	
	    for(int i = 0; i<=100; i++)
	    {
		    GameObject obj = new GameObject();
		    MeshRenderer renderer = new MeshRenderer();
		    renderer.Mesh = mesh;
		    renderer.Material = material;
		    obj.AddComponent(renderer);
		    SceneManager.ActiveScene.Add(obj);
	    }
	    Count += 100;
    }
}