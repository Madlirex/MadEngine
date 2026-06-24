using MadEditor;
using MadEngine.Core;

namespace DemoProject;

public class EditorTestComp : Component
{
    public override void EditorStart()
    {
        MeshRenderer mesh = GameObject.AddComponent<MeshRenderer>()!;
        mesh.Material.DiffuseTexture = new Texture(AssetManager.AssetsPath + "Textures/house.jpg");
    }
}