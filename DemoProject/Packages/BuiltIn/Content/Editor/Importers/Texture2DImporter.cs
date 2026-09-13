using System;
using System.Text.Json.Nodes;
using MadEngine;
using MadEngine.Core;

namespace MadEditor;

public class Texture2DImporter : DefaultAssetImporter<Texture2D>
{
    public override string Name => "TextureImporter";
    public override string[] Extensions => [".tex"];
}