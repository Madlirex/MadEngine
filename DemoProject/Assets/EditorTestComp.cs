using MadEditor;
using MadEngine;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;


public class EditorTestComp : Component
{
    public int Count = 0;
    public Mesh? mesh = null;
    public Material? material = null;

    public override void EditorStart()
    {
	    Debug.Log("EditorTestComp Start");
	    Debug.LogWarning("hola");
	    Debug.LogError("ou no");
    }

    public override void EditorUpdate(float deltaTime)
    {
	    if (mesh == null || material == null)
	    {
		    Debug.LogError("No mesh");
		    Debug.LogWarning("e");
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