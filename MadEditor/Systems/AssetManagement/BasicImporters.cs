using MadEngine.Core;

namespace MadEditor;

public class TextureAsset(Texture texture) : Asset
{
    public Texture Texture = texture;
    public override Type AssetType => typeof(Texture);
}

public class TextureImporter : AssetImporter<TextureAsset>
{
    public override string Name => "TextureImporter";
    public override IReadOnlyList<string> Extensions => [".jpg", ".png", ".jpeg"];

    public override TextureAsset Load(string path)
    {
        TextureAsset texture = new TextureAsset(new Texture(path));
        texture.Name = Path.GetFileNameWithoutExtension(path);
        
        return texture;
    }

    public override TextureAsset Load(AssetMeta meta)
    {
        TextureAsset texture = new TextureAsset(new Texture(meta.Path))
        {
            Guid = meta.Guid,
            Name = meta.Name
        };
        
        return texture;
    }

    public override void Save(TextureAsset asset, string path)
    {
        
    }
}