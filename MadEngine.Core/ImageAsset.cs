namespace MadEngine.Core;

public class ImageAsset : Asset
{
    protected override string NameInternal { get; set; } = "NewImage";
    [DoNotSave] public string Path => AbsolutePath;
}