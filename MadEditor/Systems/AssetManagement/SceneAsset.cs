using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class SceneAsset(Scene scene) : Asset
{
    public override Type AssetType => typeof(Scene);
    public Scene Scene = scene;
}

public class SceneImporter : AssetImporter<SceneAsset>
{
    public override string Name => "SceneImporter";
    public override IReadOnlyList<string> Extensions => [".madscene"];
    public override SceneAsset Load(string path)
    {
        throw new NotImplementedException();
    }

    public override SceneAsset Load(AssetMeta meta)
    {
        throw new NotImplementedException();
    }

    public override void Save(SceneAsset asset, string path)
    {
        
    }
}