namespace MadEditor;

public interface IAssetImporter
{
    public string Name { get; }
    public IReadOnlyList<string> Extensions { get; }
    
    public Asset Load(string path);
    public Asset Load(AssetMeta meta);
}