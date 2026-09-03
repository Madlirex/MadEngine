using MadEditor;
using MadEngine;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;


public class EditorTestComp : Component
{
    public int Count = 0;
    public Mesh? mesh = null;
    public Material? material = null;

    public override void EditorUpdate(float deltaTime)
    {
	if(mesh == null || material == null) return;
	
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