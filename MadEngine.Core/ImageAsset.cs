using MadEngine.Core;

namespace DefaultNamespace;

public class ImageAsset : Asset
{
    public override string Name { get; set; } = "NewImage";
    [DoNotSave] public string PathToImage => AbsolutePath;
}