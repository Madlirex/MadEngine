using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class SceneImporter : AssetImporter<Scene>
{
    public override string Name => "SceneImporter";
    public override IReadOnlyList<string> Extensions => [".madscene"];
    public override Scene Load(string path)
    {
        throw new NotImplementedException();
    }

    public override Scene Load(AssetMeta meta)
    {
        throw new NotImplementedException();
    }

    public override void Save(Scene asset, string path)
    {
        
    }
}