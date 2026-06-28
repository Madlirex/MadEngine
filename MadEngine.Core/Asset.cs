namespace MadEngine.Core;

public abstract class Asset : MadObject
{
    public override string Name { get; set; } = "NewAsset";
    public string Path = "";
    public string Extension = ".asset";
    
    public Asset()
    {
        Register();
    }

    public void Register()
    {
        Console.WriteLine("registered");
        AssetRegistry.RegisterAsset(this);
    }
}