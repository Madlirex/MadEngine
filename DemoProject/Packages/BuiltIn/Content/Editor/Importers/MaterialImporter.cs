using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public class MaterialImporter : DefaultAssetImporter<Material>
{
    public override string Name => "MaterialImporter";
    public override string[] Extensions => [".mat"];
}