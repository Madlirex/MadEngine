namespace MadEngine.Core;

public static class MaterialSystem
{
    public static IReadOnlyList<Material> Materials => _materials;

    private static List<Material> _materials = [];

    public static void RegisterMaterial(Material material)
    {
        _materials.Add(material);
    }

    public static void UnregisterMaterial(Material material)
    {
        _materials.Remove(material);
    }
}