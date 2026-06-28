using MadEngine.Core;

namespace MadEditor;

public class TextureImporter : AssetImporter<Texture>
{
    public override string Name => "TextureImporter";
    public override IReadOnlyList<string> Extensions => [".jpg", ".png", ".jpeg"];

    public override Texture Load(string path)
    {
        Texture texture = new Texture(path);
        texture.Name = Path.GetFileNameWithoutExtension(path);
        
        return texture;
    }

    public override Texture Load(AssetMeta meta)
    {
        Texture texture = new Texture(meta.Path)
        {
            Guid = meta.Guid,
            Name = meta.Name
        };
        
        return texture;
    }

    public override void Save(Texture asset, string path)
    {
        
    }
}