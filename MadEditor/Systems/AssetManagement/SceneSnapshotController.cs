using System.Text.Json.Nodes;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public static class SceneSnapshotController
{
    private static JsonNode? _cachedState;
    public static bool HasSnapshot => _cachedState != null;

    public static void TakeSnapshot(Scene scene)
    {
        SceneImporter? importer = (SceneImporter?)ImporterRegistry.GetImporter(typeof(Scene));
        if (importer == null) return;

        _cachedState = importer.SaveToJson(scene);
    }

    public static Scene RestoreSnapshot()
    {
        if (_cachedState == null) return new Scene();
        SceneImporter? importer = (SceneImporter?)ImporterRegistry.GetImporter(typeof(Scene));

        importer?.InstantiateFromJson(_cachedState);
        return importer?.ImportFromJson(_cachedState) ?? new Scene();
    }
}