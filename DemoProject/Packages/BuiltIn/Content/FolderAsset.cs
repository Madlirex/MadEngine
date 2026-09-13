using MadEngine.Core;

namespace MadEngine;

public class FolderAsset : Asset
{
    protected override string ExtensionInternal { get; set; } = "";
    protected override string NameInternal { get; set; } = "NewFolder";
}