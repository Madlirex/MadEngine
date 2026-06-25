using MadEngine.Core;

namespace MadEditor;

public class TextureAsset(Texture texture) : Asset
{
    public Texture Texture = texture;
    public override Type AssetType => typeof(Texture);
}

public class TextureImporter : IAssetImporter
{
    public string Name => "TextureImporter";
    public IReadOnlyList<string> Extensions => [".jpg", ".png", ".jpeg"];
    
    public Asset Load(string path)
    {
        TextureAsset texture = new TextureAsset(new Texture(path));
        texture.Name = Path.GetFileNameWithoutExtension(path);
        
        return texture;
    }

    public Asset Load(AssetMeta meta)
    {
        TextureAsset texture = new TextureAsset(new Texture(meta.Path))
        {
            Guid = meta.Guid,
            Name = meta.Name
        };
        
        return texture;
    }
}