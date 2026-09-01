using MadEditor;
using MadEngine;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;


public class EditorTestComp : Component
{
    public Dictionary<string, int> vertices = [];

    public int[] more = [];
    /*
    public override void EditorUpdate(float deltaTime)
    {
        Console.WriteLine(SceneManager.ActiveScene.GameObjects.Count);
        GameObject obj = new GameObject();
        obj.AddComponent<MeshRenderer>();
        SceneManager.ActiveScene.Add(obj);
    }*/
}