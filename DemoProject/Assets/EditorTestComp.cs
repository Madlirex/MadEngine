using MadEditor;
using MadEngine;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;


public class EditorTestComp : Component
{
    public Dictionary<string, Vertex> vertices = [];
    /*
    public override void EditorUpdate(float deltaTime)
    {
        Console.WriteLine(SceneManager.ActiveScene.GameObjects.Count);
        GameObject obj = new GameObject();
        obj.AddComponent<MeshRenderer>();
        SceneManager.ActiveScene.Add(obj);
    }*/
}